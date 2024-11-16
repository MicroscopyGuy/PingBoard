using Google.Rpc.Context;
using Microsoft.EntityFrameworkCore;
using PingBoard.Database.Models;
using PingBoard.Database.Utilities;

namespace PingBoard.Services;
using System.Collections.Immutable;
using System.Threading.Channels;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using PingBoard.Endpoints;

/// <summary>
/// A class which represents the service offerings of the PingBoard backend, and defines
/// the central stream through which ServerEvents are sent to the frontend.
/// </summary>
public class PingBoardService : global::PingBoardService.PingBoardServiceBase
{
    private PingMonitoringJobManager _pingMonitoringJobManager;
    private readonly IImmutableList<IChannelReaderAdapter> _serverEventChannelReaders;
    private readonly ILogger<PingBoardService> _logger;
    private readonly CrudOperations _crudOperations;
    
    
    public PingBoardService(PingMonitoringJobManager pingMonitoringJobManager, CrudOperations crudOperations, 
                            [FromKeyedServices("ServerEventChannelReaders")] IImmutableList<IChannelReaderAdapter> serverEventChannelReaders,
                            ILogger<PingBoardService> logger)
    {
        _pingMonitoringJobManager = pingMonitoringJobManager;
        _crudOperations = crudOperations;
        _serverEventChannelReaders = serverEventChannelReaders;
        _logger = logger;
    }
    
    /// <summary>
    /// This function directly handles each request from the frontend to begin pinging a target,
    /// by directing the PingMonitoringJobManager to StartPinging, if it isn't already.
    /// </summary>
    /// <param name="request">The target the user wishes to ping.</param>
    /// <param name="context">Represents the context of a server-side call.</param>
    /// <returns>An empty task.</returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<Empty> StartPinging(StartPingingRequest request, ServerCallContext context)
    {
        try
        {
            if (_pingMonitoringJobManager.IsPinging())
            {
                _logger.LogDebug($"PingBoardService: StartPinging: Was already pinging {request.Target.Target}");
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Was already pinging!"));
            }

            _logger.LogDebug($"PingBoardService StartPinging: {request.Target.Target}");
            _pingMonitoringJobManager.StartPinging(request.Target.Target);
            return new Empty();
        }
        
        catch (RpcException rpcException)
        {
            _logger.LogError($"PingBoardService: StartPinging: {rpcException}");
            Console.WriteLine($"{rpcException}");
        }

        catch (Exception e)
        {
            _logger.LogError($"PingBoardService: StartPinging: {e}");
            Console.WriteLine($"{e}");
        }

        return new Empty();
    }

    /// <summary>
    /// This function directly handles each request from the frontend to cease the pinging of a target,
    /// by directing the PingMonitoringJobManager to StopPinging, if it is already.
    /// </summary>
    /// <param name="request">The target the user wishes to cease sending pings to.</param>
    /// <param name="context">Represents the context of a server-side call.</param>
    /// <returns>An empty task.</returns>
    /// <exception cref="RpcException"></exception>
    public override async Task<Empty> StopPinging(Empty request, ServerCallContext context)
    {
        try
        {
            if (!_pingMonitoringJobManager.IsPinging())
            {
                _logger.LogDebug("PingBoardService: StopPinging: Was not pinging");
                // case covered, gRPC will cover this and return to user as friendly 
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Wasn't pinging!"));
            }

            _logger.LogDebug($"PingBoardService: StopPinging");
            await _pingMonitoringJobManager.StopPingingAsync();
            return new Empty();
        }

        catch (RpcException rpcException)
        {
            _logger.LogError($"PingBoardService: StopPinging: {rpcException}");
            Console.WriteLine($"{rpcException}");
        }

        catch (Exception e)
        {
            _logger.LogError($"PingBoardService: StopPinging: {e}");
            Console.WriteLine($"{e}");
        }

        return new Empty();
    }
    
    /* Consider renaming this to SendLatestServerEvents: pluralize it, and clarify direction of communication */
    /// <summary>
    /// Streams ServerEvents to the FrontEnd, one at a time. See the ServerEvent definition in protos/service.proto
    /// for a comprehensive list of which ServerEvents are supported. 
    /// </summary>
    /// <param name="request">Empty, since no user paramaters are needed./param>
    /// <param name="responseStream">The writer that writes ServerEvents to the stream.</param>
    /// <param name="context">Represents the context of a server-side call.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public override async Task GetLatestServerEvent(Empty request, IServerStreamWriter<ServerEvent> responseStream,
        ServerCallContext context)
    {
        _logger.LogDebug("GetLatestServerEvent: Starting API call");
        while (!context.CancellationToken.IsCancellationRequested)
        {
            var readyTaskReader = await GetReadyChannelReaderAdapter(context.CancellationToken);

            // the boolean result of Invoke indicates if something was read or not
            ServerEvent? eventToSend;
            while ((eventToSend = readyTaskReader.ReadNextServerEvent()) != null)
            {
                await responseStream.WriteAsync(eventToSend, context.CancellationToken); 
            }
        }
    }
    
    /// <summary>
    /// A helper method for GetLatestServerEvent which figures out which IChannelReaderAdapter has a message,
    /// and then returns it so GetLatestServerEvent can send the ServerEvent the IChannelReaderAdapter
    /// needs to tell the frontend about.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>The IChannelReaderAdapter that has a new message</returns>
    /// <exception cref="TaskCanceledException"></exception>
    private async Task<IChannelReaderAdapter> GetReadyChannelReaderAdapter(CancellationToken cancellationToken)
    {
        List<Task<bool>> channelReaderTasks = new List<Task<bool>>();

        foreach (var reader in _serverEventChannelReaders)
        {
            channelReaderTasks.Add(reader.WaitToReadAsync(cancellationToken));
        }

        // find which task completed, and then find the reader for that task
        var readyTask = await Task.WhenAny(channelReaderTasks);
        
        // this would mean that all of the tasks are also canceled, so it's safe to simply return
        if (readyTask.IsCanceled){
            throw new TaskCanceledException("Cancellation was requested");
        }

        // unclear exactly how, when, or if this could even happen. Due to this, the application should be treated 
        // as corrupted or unusable.
        if (readyTask.IsFaulted){
            string failFastMsg = """
                                 One or more Backend event-processing-channels encountered a faulted state for an unknown reason.
                                 Application is now in an unusable state and must be restarted.
                                 """;

            _logger.LogCritical(failFastMsg + "\n" + readyTask.Exception);
            
            // kills the application immediately, logs to the OS crash logs
            Environment.FailFast(failFastMsg, readyTask.Exception);
        }
        
        var readyTaskReader = _serverEventChannelReaders[channelReaderTasks.IndexOf(readyTask)];
        return readyTaskReader;
    }

    /*
    // give this a PaginationToken as well, add to protobuff
    public override async Task<ListPingsResponse> ListPings(ListPingsRequest request, ServerCallContext context)
    {
        var response = new ListPingsResponse();
        var results = _crudOperations(
            request.StartingTime,
            request.EndingTime,
            request.PingTarget,
            request.Metric,
            request.Statistic,
            context.CancellationToken,
            request.Quantum);

    }*/
    
    
    public override async Task<ListAnomaliesResponse> ListAnomalies(ListAnomaliesRequest request, ServerCallContext context)
    {
        _logger.LogDebug("GetLatestServerEvent: Starting API call");
        
        // create a PaginationToken if the UI did not supply one
        DateTime startTime = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(request.PaginationToken))
        {
            var suppliedToken = PaginationToken<DateTime>.FromApiFormat(request.PaginationToken, "ListAnomalies");
            startTime = suppliedToken.Token;
        }
        
        var response = new ListAnomaliesResponse();
        var anomalies = await _crudOperations.ListAnomaliesAsync(
            startTime,
            request.NumberRequested+1,
            context.CancellationToken,
            request.PingTarget?.Target
        );

        if (anomalies.Count == request.NumberRequested+1)
        {
            var nextCursor = anomalies[(int)request.NumberRequested].Start.ToDateTime();
            response.PaginationToken = PaginationToken<DateTime>.ToApiFormat(nextCursor, "ListAnomalies");
            anomalies.RemoveAt((int) (request.NumberRequested));
        }
        
        else // if there aren't enough anomalies for a next page
        {
            response.PaginationToken = "";
        }
        
        response.Anomalies.Add(anomalies);
        
        return response;
    }

    public override async Task<ShowPingsResponse> ShowPings(ShowPingsRequest request, ServerCallContext context)
    {
        _logger.LogDebug("ShowPings: Starting API call");
        try
        {
            var response = await _crudOperations.ShowPingsAsync(request.StartingTime.ToDateTime(),
                request.EndingTime.ToDateTime(),
                request.Target.ToString(), context.CancellationToken);
            _logger.LogDebug($"ShowPing: number of results: {response.Pings.Count}");
            return response;
        }
        
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            throw;
        }
    }
}