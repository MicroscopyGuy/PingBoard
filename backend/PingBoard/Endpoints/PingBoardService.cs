using System.Threading.Channels;

namespace PingBoard.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
//using "PingBoard/protos/service.proto";

public class PingBoardService : global::PingBoardService.PingBoardServiceBase
{
    private readonly ILogger<PingBoardService> _logger;
    private PingMonitoringJobManager _pingMonitoringJobManager;
    private PingStatusIndicator _pingStatusIndicator;
    public PingBoardService(PingMonitoringJobManager pingMonitoringJobManager, PingStatusIndicator pingStatusIndicator,
        ILogger<PingBoardService> logger)
    {
        _pingMonitoringJobManager = pingMonitoringJobManager;
        _pingStatusIndicator = pingStatusIndicator;
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
    
    public override async Task GetPingingStatus(Empty request, IServerStreamWriter<PingStatusMessage> responseStream, ServerCallContext context)
    {
        while (await _pingStatusIndicator.Reader.WaitToReadAsync(context.CancellationToken))
        {
            while (_pingStatusIndicator.Reader.TryRead(out PingStatusMessage message))
            {
                await responseStream.WriteAsync(message, context.CancellationToken);
            }
        }
        
        //return base.GetPingingStatus(request, responseStream, context);
    }
}