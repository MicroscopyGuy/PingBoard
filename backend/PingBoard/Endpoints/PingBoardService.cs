namespace PingBoard.Services;
using System.Collections.Immutable;
using System.Threading.Channels;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
//using "PingBoard/protos/service.proto";

public class PingBoardService : global::PingBoardService.PingBoardServiceBase
{
    
    private PingMonitoringJobManager _pingMonitoringJobManager;
    private ServerEventEmitter _serverEventEmitter;
    private readonly IImmutableList<object> _serverEventChannelReaders;
    private readonly ILogger<PingBoardService> _logger;
    
    public PingBoardService(PingMonitoringJobManager pingMonitoringJobManager, ServerEventEmitter serverEventEmitter,
                            [FromKeyedServices ("ServerEventChannelReaders")] IImmutableList<object> serverEventChannelReaderList,
                            ILogger<PingBoardService> logger)
    {
        _pingMonitoringJobManager = pingMonitoringJobManager;
        _serverEventEmitter = serverEventEmitter;
        _serverEventChannelReaders = serverEventChannelReaderList;
        _logger = logger;
    }
    
    public override async Task<Empty> StartPinging(PingTarget request, ServerCallContext context)
    {
        try
        {
            if (_pingMonitoringJobManager.IsPinging())
            {
                _logger.LogDebug($"PingBoardService: StartPinging: Was already pinging {request.Target}");
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Was already pinging!"));
            }

            _logger.LogDebug($"PingBoardService StartPinging: {request.Target}");
            _pingMonitoringJobManager.StartPinging(request.Target);
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
    public override async Task GetLatestServerEvent(Empty request, IServerStreamWriter<ServerEvent> responseStream,
        ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            
            var readyTaskReader = await GetChannelWithMessage(context.CancellationToken);

            if (readyTaskReader == null)
            {
                throw new InvalidOperationException("The reader for the Channel task is null");
            }
            
            var readyTaskReaderType = readyTaskReader.GetType();

            // finds method that waits until there is something to read, and then reads it
            var readMethod = readyTaskReaderType.GetMethod(nameof(ChannelReader<object>.TryRead))!;
            

            object[] parameters = new object[] { null };

            // the boolean result of Invoke indicates if something was read or not
            while ((bool)readMethod.Invoke(readyTaskReader, parameters)!)
            {
                var message = parameters[0]; // this is what was read from the read method
                var serverEvent = new ServerEvent();
                serverEvent.PingOnOffToggle = new ServerEvent.Types.PingOnOffToggle();
                var serverEventType = serverEvent.GetType();
                var props = serverEventType.GetProperties();

                foreach (var prop in props)
                {
                    var eventType = prop.PropertyType;
                    if (message.GetType() == eventType)
                    {
                        prop.SetValue(serverEvent, message);
                        break;
                    }
                }

                await responseStream.WriteAsync(serverEvent, context.CancellationToken);
            }
        }
    }
    
    private async Task<object> GetChannelWithMessage(CancellationToken cancellationToken)
    {
        List<Task<bool>> channelReaderTasks = new List<Task<bool>>();

        foreach (var reader in _serverEventChannelReaders)
        {
            // need access to the type of the reader first to get access to the read method
            var type = reader.GetType();
            
            // finds method that waits until there is something to read, and then reads it
            var waitMethod = type.GetMethod(nameof (ChannelReader<object>.WaitToReadAsync))!;
            var readerTask = waitMethod.Invoke(reader, [cancellationToken])!;
            if (readerTask is ValueTask<bool>)
            {
                return reader;
            }
            
            channelReaderTasks.Add((Task<bool>)readerTask);
            
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
}