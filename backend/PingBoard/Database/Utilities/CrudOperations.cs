namespace PingBoard.Database.Utilities;

using System.Text.Json;
using Google.Rpc.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PingBoard.Database.Models;
using PingBoard.Probes.NetworkProbes;
using PingBoard.Services;
using Probes.NetworkProbes.Common;
using Protos;

/// <summary>
/// An extension class for PingInfoContext that defines some APIs for interacting with the PingBoard database
/// </summary>
public class CrudOperations
{
    private IDbContextFactory<ProbeResultsContext> _probeResultsContextFactory;
    private ILogger<CrudOperations> _logger;

    public CrudOperations(
        IDbContextFactory<ProbeResultsContext> probeResultsContextFactory,
        ILogger<CrudOperations> logger
    )
    {
        _probeResultsContextFactory = probeResultsContextFactory;
        _logger = logger;
    }

    /*
    /// <summary>
    /// Retrieves the next N number of records from the database from a certain point in time, returned
    /// as a list of PingGroupSummaryPublic objects
    /// </summary>
    /// <param name="dbContext">Represents a Unit of Work with the PingInfo database</param>
    /// <param name="startingTime">The initial (inclusive) time from which the offset N, should be applied</param>
    /// <param name="numToGet">The number of records to retrieve from the initial (inclusive) time</param>
    /// <param name="cancellationToken"> </param>
    /// <returns></returns>
    public async Task<List<PingGroupSummaryPublic>> ListPingsAsync(
        DateTime startingTime,
        uint numToGet,
        CancellationToken cancellationToken,
        string target = ""
    )
    {
        await using var pingInfoContext = await _pingInfoContextFactory.CreateDbContextAsync(
            cancellationToken
        );

        var summaries = pingInfoContext
            .Summaries.Where(s => s.Start >= startingTime)
            .Where(s => (target != "") ? s.Target == target : s.Target != null)
            .Take((int)numToGet)
            .Select(s => PingGroupSummary.ToApiModel(s))
            .OrderByDescending(s => s.Start)
            .ToList();

        return summaries;
    }*/


    /// <summary>
    /// Retrieves the next N number of records from the database from a certain point in time, returned
    /// as a list of PingGroupSummaryPublic objects
    /// </summary>
    /// <param name="startingTime">The initial (inclusive) time from which the offset N, should be applied</param>
    /// <param name="numToGet">The number of records to retrieve from the initial (inclusive) time</param>
    /// <param name="cancellationToken"> </param>
    /// <param name="target">An optional parameter for the retrieval of anomalies matching a given target </param>
    /// <returns></returns>
    public async Task<List<PingResultPublic>> ListAnomaliesAsync(
        DateTime startingTime,
        uint numToGet,
        CancellationToken cancellationToken,
        string? target = ""
    )
    {
        await using var probeInfoContext = await _probeResultsContextFactory.CreateDbContextAsync(
            cancellationToken
        );

        var resultsQuery = probeInfoContext.ProbeResults.Where(s => s.Start <= startingTime);

        if (target is not null)
        {
            resultsQuery = resultsQuery.Where((s) => s.Target == target);
        }

        var results = resultsQuery
            .Where(s => s.Anomaly)
            .Take((int)numToGet)
            .OrderByDescending(s => s.Start)
            .ToList();

        var convertedResults = results
            .Select(s => PingProbeResult.ToApiModel((PingProbeResult)s))
            .ToList();

        return convertedResults;
    }

    /*
    public async Task<ShowPingsResponse> ShowPingsAsync(
        DateTime startingTime,
        DateTime endingTime,
        string pingTarget,
        CancellationToken cancellationToken
    )
    {
        await using var pingInfoContext = await _pingInfoContextFactory.CreateDbContextAsync(
            cancellationToken
        );
        var results = pingInfoContext
            .Summaries.Where(s => /*s.Start >= startingTime && s.End < endingTime &&
                s.Target == pingTarget
            )
            .Select(s => PingGroupSummary.ToApiModel(s))
            .ToList();

        Console.WriteLine($"ShowPingsAsync: number of results: {results.Count}");
        var response = new ShowPingsResponse();
        response.Pings.Add(results);

        return response;
    } */

    public async Task InsertProbeResult(ProbeResult result, CancellationToken cancellationToken)
    {
        try
        {
            await using var _probeDbContext =
                await _probeResultsContextFactory.CreateDbContextAsync(cancellationToken);

            result.ProbeSubtypeData = JsonSerializer.Serialize(result);
            _probeDbContext.Add(result);
            _probeDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(
                "CrudOperations: InsertProbeResult: Insertion of ProbeResult failed. {e}",
                e
            );
            throw;
        }
    }

    /*
    public async Task<List<ListPingsResponse>> ListPingsAsync(DateTime startingTime, DateTime endingTime,
        string pingTarget, string metric, string statistic, CancellationToken cancellationToken, TimeSpan? quantum)
    {
        await using var pingInfoContext = await _pingInfoContextFactory.CreateDbContextAsync(cancellationToken);
        long quantumTicks = quantum != null ? ((TimeSpan)quantum).Ticks : 1;

        var datapoints = pingInfoContext.Summaries
            .Where(s => s.Start >= startingTime && s.End < endingTime)
            .GroupBy(s => s.Start.Ticks / quantumTicks)
            .ToDictionary(grp => grp.Key, grp => grp.ToList());
        
        

        /*
         * var query = petsList.GroupBy(
        pet => Math.Floor(pet.Age),
        pet => pet.Age,
        (baseAge, ages) => new
        {
            Key = baseAge,
            Count = ages.Count(),
            Min = ages.Min(),
            Max = ages.Max()
        });

         series.GroupBy (s => s.timestamp.Ticks / TimeSpan.FromHours(1).Ticks)
        .Select (s => new {
            series = s
            ,timestamp = s.First ().timestamp
            ,average = s.Average (x => x.value )
        }).Dump();
         
    } */
}
