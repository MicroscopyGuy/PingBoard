using Grpc.Core;

namespace PingBoard.Tests;

public class ListAnomaliesAsyncTest : BaseIntegrationTest
{
    /*
    [Fact]
    public async Task OnApiCall_GetsResults()
    {
        var apiCallResults = await Client.ListAnomaliesAsync(
            new ListAnomaliesRequest()
            {
                NumberRequested = 10,
                PaginationToken = ""
            });
        Assert.NotNull(apiCallResults);
        Assert.Equal(10, apiCallResults.Anomalies.Count);
    }
    
    [Fact]
    public async Task OnFloodApiCalls_GetsResults()
    {
        int calls = 1000;

        for (int i = 0; i < calls; i++)
        {
            var apiCallResults = await Client.ListAnomaliesAsync(
                new ListAnomaliesRequest()
                {
                    NumberRequested = 10,
                    PaginationToken = ""
                });
            Assert.NotNull(apiCallResults);
            Assert.Equal(10, apiCallResults.Anomalies.Count);
        }
    }
    
    [Fact]
    public async Task OnFloodOfPaginatedApiCalls_GetsResults()
    {
        int calls = 1000;
        string paginationToken = "";
        for (int i = 0; i < calls; i++)
        {
            var timeout = Task.Delay(TimeSpan.FromSeconds(3));
            var currentApiTask = Client.ListAnomaliesAsync(
                new ListAnomaliesRequest()
                {
                    NumberRequested = 10,
                    PaginationToken = paginationToken
                }).ResponseAsync;
            
            var firstResult = await Task.WhenAny(timeout, currentApiTask);
            Assert.Equal(currentApiTask, firstResult);
            Assert.NotNull(currentApiTask.Result);
            paginationToken = currentApiTask.Result.PaginationToken;
        }
    } */
}