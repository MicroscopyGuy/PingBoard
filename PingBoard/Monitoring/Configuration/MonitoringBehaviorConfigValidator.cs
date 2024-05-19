namespace PingBoard.Monitoring.Configuration;
using PingBoard.Pinging.Configuration;
using Microsoft.Extensions.Options;
using FluentValidation;
using MonitoringLimits = PingBoard.Monitoring.Configuration.MonitoringBehaviorConfigLimits;


public class MonitoringBehaviorConfigValidator : AbstractValidator<MonitoringBehaviorConfig>{
    public MonitoringBehaviorConfigValidator(IOptions<PingingBehaviorConfig> pingingBehaviorConfig){
        RuleFor(mbc => mbc.OutageAfterTimeouts)
            .InclusiveBetween(MonitoringLimits.FewestAllowedConsecutiveTimeoutsBeforeOutage, 
                              MonitoringLimits.MostAllowedConsecutiveTimeoutsBeforeOutage)
            .WithMessage("""
                            For the sake of program responsiveness to outages, the OutageAfterTimeout property
                            must be kept low. 
                        """);
        
        RuleFor(mbc => mbc.OutageAfterTimeouts)
            .GreaterThanOrEqualTo(pingingBehaviorConfig.Value.ReportBackAfterConsecutiveTimeouts)
            .WithMessage($"""
                            The program cannot declare an outage at a *lower* number of timeouts than the number configured
                            in appsettings.json for ReportBackAfterConsecutiveTimeouts (currently set to 
                            {pingingBehaviorConfig.Value.ReportBackAfterConsecutiveTimeouts})
                          """);
    }
}


