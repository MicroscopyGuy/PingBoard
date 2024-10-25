using System.Text.Json;

namespace PingBoard.Database.Utilities;
using Microsoft.Extensions.Options;
using PingBoard.Pinging.Configuration;
using Microsoft.EntityFrameworkCore;
using PingBoard.Database.Models;
    
/// <summary>
/// An extension class for PingInfoContext that defines some APIs for interacting with the PingBoard database
/// </summary>
public class CrudOperations
{
    private IDbContextFactory<PingInfoContext> _pingInfoContextFactory;
    private PingingThresholdsConfig _pingingThresholds;
    private ILogger<CrudOperations> _logger;

    public CrudOperations(IDbContextFactory<PingInfoContext> pingInfoContextFactory,
                          IOptions<PingingThresholdsConfig> pingingThresholds,
                          ILogger<CrudOperations> logger)
    {
        _pingInfoContextFactory = pingInfoContextFactory;
        _pingingThresholds = pingingThresholds.Value;
        _logger = logger;
    }
    
    /// <summary>
    /// Inserts a PingGroupSummary (private) into the database
    /// </summary>
    /// <param name="summary">The PingGroupSummary objects whose values should be stored in the DB</param>
    /// <param name="cancellationToken"> </param>
    public async Task InsertPingGroupSummaryAsync(PingGroupSummary summary, CancellationToken cancellationToken)
    {
        try
        {
            await using var pingInfoContext = await _pingInfoContextFactory.CreateDbContextAsync(cancellationToken);
            pingInfoContext.Add(summary);
            pingInfoContext.SaveChanges();
        }

        catch (Exception e)
        {
            _logger.LogError("CrudOperations: InsertPingGroupSummaryAsync: Insertion of PingGroupSummary failed. {e}", e);
            throw;
        }
        
    }
    
    /// <summary>
    /// Retrieves the next N number of records from the database from a certain point in time, returned
    /// as a list of PingGroupSummaryPublic objects
    /// </summary>
    /// <param name="dbContext">Represents a Unit of Work with the PingInfo database</param>
    /// <param name="startingTime">The initial (inclusive) time from which the offset N, should be applied</param>
    /// <param name="numToGet">The number of records to retrieve from the initial (inclusive) time</param>
    /// <param name="cancellationToken"> </param>
    /// <returns></returns>
    public async Task<List<PingGroupSummaryPublic>> ListPingsAsync( DateTime startingTime, 
        uint numToGet, CancellationToken cancellationToken, string target = "")
    {
        await using var pingInfoContext = await _pingInfoContextFactory.CreateDbContextAsync(cancellationToken);
        
        var summaries = pingInfoContext
            .Summaries
            .Where(s => s.Start >= startingTime)
            .Where(s => (target != "") ? s.Target == target : s.Target != null)
            .Take((int) numToGet)
            .Select(s => PingGroupSummary.ToApiModel(s))
            .OrderByDescending(s => s.Start)
            .ToList();
        
        return summaries;
    }
    
    /// <summary>
    /// Retrieves the next N number of records from the database from a certain point in time, returned
    /// as a list of PingGroupSummaryPublic objects
    /// </summary>
    /// <param name="startingTime">The initial (inclusive) time from which the offset N, should be applied</param>
    /// <param name="numToGet">The number of records to retrieve from the initial (inclusive) time</param>
    /// <param name="cancellationToken"> </param>
    /// <param name="target">An optional parameter for the retrieval of anomalies matching a given target </param>
    /// <returns></returns>
    public async Task<List<PingGroupSummaryPublic>> ListAnomaliesAsync( DateTime startingTime, 
        uint numToGet, CancellationToken cancellationToken, string? target = "")
    {
        await using var pingInfoContext = await _pingInfoContextFactory.CreateDbContextAsync(cancellationToken);

        var summaryQuery = pingInfoContext
            .Summaries
            .Where(s => s.Start <= startingTime);

        if (target != null)
        {
            summaryQuery = summaryQuery.Where((s) => s.Target == target);
        }
        
        
        var summaries = summaryQuery
            .Where(s => s.MinimumPing > _pingingThresholds.MinimumPingMs ||
                                        s.AveragePing > _pingingThresholds.AveragePingMs ||
                                        s.MaximumPing > _pingingThresholds.MaximumPingMs ||
                                        s.Jitter > _pingingThresholds.JitterMs ||
                                        s.TerminatingIPStatus != null)
            .Take((int) numToGet)
            .OrderByDescending(s => s.Start)
            .ToList();

        var convertedSummaries = summaries
            .Select(s => PingGroupSummary.ToApiModel(s))
            .ToList();

        return convertedSummaries;
    }
 
}