using Google.Protobuf.WellKnownTypes;
namespace PingBoard.Services;
using Grpc.Core;

public class PingBoardService : global::PingBoardService.PingBoardServiceBase
{
    private readonly ILogger<PingBoardService> _logger;
    private PingMonitoringJobManager _pingMonitoringJobManager;
    
    public PingBoardService(PingMonitoringJobManager pingMonitoringJobManager,
        ILogger<PingBoardService> logger)
    {
        _pingMonitoringJobManager = pingMonitoringJobManager;
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
            _pingMonitoringJobManager.StopPinging();
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
}