using Microsoft.AspNetCore.Mvc;
using PingBoard.Apis.ProbeData;
using PingBoard.Database.Utilities;
using PingBoard.Probes.NetworkProbes.Ping;

public static class Anomalies
{
    public static void MapAnomalyApis(this WebApplication app)
    {
        var anomalies = app.MapGroup("ProbeData");
    }

    /*
     * message ListAnomaliesRequest{
      uint32 numberRequested = 1;
      string paginationToken = 2;
      optional PingTarget pingTarget = 3;
    }
    
    message ListAnomaliesResponse{
      repeated PingResultPublic anomalies = 1;
      string paginationToken = 2;
    }
     */
    [Consumes("application/json")]
    private static async Task<IResult> ListAnomalies(
        [FromServices] CrudOperations crudOperations,
        [FromQuery] string? paginationToken,
        [FromQuery] uint numberRequested,
        CancellationToken cancellationToken,
        [FromServices] Logger<PingResultPublic> logger,
        [FromQuery] string? target = null
    )
    {
        logger.LogDebug("ListAnomalies: Starting API call");
        // create a PaginationToken if the UI did not supply one
        DateTime startTime = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(paginationToken))
        {
            var suppliedToken = PaginationToken<DateTime>.FromApiFormat(
                paginationToken,
                "ListAnomalies"
            );
            startTime = suppliedToken.Token;
        }
        var response = new ListAnomaliesResponse();
        var anomalies = await crudOperations.ListAnomaliesAsync(
            startTime,
            numberRequested + 1,
            cancellationToken,
            target
        );

        if (anomalies.Count == numberRequested + 1)
        {
            var nextCursor = anomalies[(int)numberRequested].Start;
            response.PaginationToken = PaginationToken<DateTime>.ToApiFormat(
                nextCursor,
                "ListAnomalies"
            );
            anomalies.RemoveAt((int)(numberRequested));
        }
        else // if there aren't enough anomalies for a next page
        {
            response.PaginationToken = "";
        }
        response.Anomalies = anomalies;

        return Results.Json(response);
    }
}
