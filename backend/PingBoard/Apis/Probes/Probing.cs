namespace PingBoard.Apis.Probes;

using Microsoft.AspNetCore.Mvc;
using PingBoard.Probes.NetworkProbes.Common;
using Services;

public static class Probing
{
    public static void MapProbingApis(this WebApplication app)
    {
        var probes = app.MapGroup("/Probes");
        probes
            .MapPost("", StartProbing)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);
        probes
            .MapDelete("", StopProbing)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> StartProbing(
        [FromBody] ProbeConfigAggregate probeConfig,
        [FromServices] ProbeOperationsCenter probeOperationsCenter,
        [FromServices] ILogger logger
    )
    {
        logger.LogInformation("(1) PingBoard Service: StartProbing hit");
        //var probeSchema = request.RequestJson.Parse(request.RequestJson);

        try
        {
            if (probeOperationsCenter.IsProbingActive())
            {
                logger.LogError(
                    $"PingBoardService: StartPinging: Was already probing{probeConfig.Behavior.Target}"
                );

                var problemDetails = new ProblemDetails()
                {
                    Status = 409,
                    Title = "Already probing max # of targets",
                    Type = "InsufficientProbeCapacity",
                };
                return Results.Problem(problemDetails);
            }
            probeOperationsCenter.StartProbing(probeConfig);
        }
        catch (Exception e)
        {
            logger.LogError($"PingBoardService: StartProbing: {e}");
            Console.WriteLine($"{e}");
            var problemDetails = new ProblemDetails()
            {
                Status = 500,
                Title = "An unaccounted for error occurred",
                Type = e.GetType().Name,
            };
            return Results.Problem(problemDetails);
        }

        return Results.NoContent();
    }

    public static async Task<IResult> StopProbing(
        [FromServices] ProbeOperationsCenter probeOperationsCenter,
        CancellationToken cancellationToken,
        [FromServices] ILogger logger
    )
    {
        try
        {
            if (!probeOperationsCenter.IsProbingActive())
            {
                logger.LogDebug("PingBoardService: StopProbing: Was not probing");
                var problemDetails = new ProblemDetails()
                {
                    Status = 422,
                    Title = "There are no probes that can be stopped; no probes are running.",
                    Type = "NoRunningProbes",
                };
                return Results.Problem(problemDetails);
            }

            logger.LogDebug($"PingBoardService: StopProbing");
            await probeOperationsCenter.StopProbingAsync();
        }
        catch (Exception e)
        {
            logger.LogError($"PingBoardService: StopProbing: {e}");
            Console.WriteLine($"{e}");
            var problemDetails = new ProblemDetails()
            {
                Status = 500,
                Title = "An unaccounted for error occurred while stopping a probe.",
                Type = e.GetType().Name,
            };
            return Results.Problem(problemDetails);
        }

        return Results.NoContent();
    }
}
