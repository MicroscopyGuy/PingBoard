using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;
using PingBoard.DatabaseUtilities;
using System.Net;

[ExcludeFromCodeCoverage]
public class NetworkMonitoringService : BackgroundService
{
    private readonly ILogger<IGroupPinger> _logger;
    private readonly IGroupPinger _groupPinger;
    private readonly PingingBehaviorConfig _pingingBehavior;
    private readonly PingingThresholdsConfig _pingingThresholds;
    private readonly PingingBehaviorConfigValidator _pingingBehaviorValidator;
    private readonly PingingThresholdsConfigValidator _pingingThresholdsValidator;
    private readonly DatabaseHelper _databaseHelper;

    public NetworkMonitoringService( IGroupPinger groupPinger, IOptions<PingingBehaviorConfig> pingingBehavior, IOptions<PingingThresholdsConfig> pingingThresholds,
                                     PingingBehaviorConfigValidator behaviorValidator, PingingThresholdsConfigValidator thresholdsValidator,
                                     DatabaseHelper databaseHelper, ILogger<IGroupPinger> logger){
        _pingingBehavior = pingingBehavior.Value;
        _pingingThresholds = pingingThresholds.Value;
        _logger = logger;
        _groupPinger = groupPinger;
        _pingingBehaviorValidator = behaviorValidator;
        _pingingThresholdsValidator = thresholdsValidator;
        _databaseHelper = databaseHelper;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken){

        // validate configured information
        ValidationResult thresholdsValidationResults = _pingingThresholdsValidator.Validate(_pingingThresholds);
        ValidationResult behaviorValidationResults   = _pingingBehaviorValidator.Validate(_pingingBehavior);
        
        // initialize database
        _databaseHelper.InitializeDatabase();
        
        if (thresholdsValidationResults.IsValid && behaviorValidationResults.IsValid){
            while (!stoppingToken.IsCancellationRequested)
            {
                PingGroupSummary result = await _groupPinger.SendPingGroupAsync(IPAddress.Parse("8.8.8.8"), _pingingBehavior.PingsPerCall);
                if (true /*!PingQualification.PingQualityWithinThresholds(result.PingQualityFlags) || result.TerminatingIPStatus != null*/){
                    Console.WriteLine($"MinimumPing: {result.MinimumPing} AveragePing: {result.AveragePing} " +
                                    $"MaximumPing: {result.MaximumPing} Jitter: {result.Jitter} PacketLoss: {result.PacketLoss} " +
                                    $"TerminatingIPStatus: {result.TerminatingIPStatus} EndTime: {result.End.ToString("MM:dd:yyyy:hh:mm:ss.ffff")}");
                    
                    //store the latest ping group summary

                    _databaseHelper.InsertPingGroupSummary(result);
                }

                await Task.Delay(0,stoppingToken);
            }
        }

        else{
            Console.WriteLine(thresholdsValidationResults.ToString("~"));
            Console.WriteLine(behaviorValidationResults.ToString("~"));           
        }
    }
}