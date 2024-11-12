using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace PingBoard.Tests;

public class BaseIntegrationTest : IDisposable
{
    private readonly GrpcChannel _grpcChannel;
    private readonly PingBoardService.PingBoardServiceClient _pingBoardServiceClient;
    
    public BaseIntegrationTest()
    {
        _grpcChannel = GrpcChannel.ForAddress("http://localhost:5245/PingBoardService", new GrpcChannelOptions
        {
            HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
            HttpVersion = Version.Parse("1.1")
        });
        _pingBoardServiceClient = new PingBoardService.PingBoardServiceClient(_grpcChannel);
    }

    protected PingBoardService.PingBoardServiceClient Client => _pingBoardServiceClient;
    
    public void Dispose()
    {
        _grpcChannel.Dispose();
    }
}