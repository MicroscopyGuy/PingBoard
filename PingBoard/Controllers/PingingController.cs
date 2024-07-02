namespace PingBoard.Controllers;
using PingBoard.Services;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("PingingController")]
public class PingingController : ControllerBase
{
    private static ILogger<PingingController> _logger;
    private static PingMonitoringJobManager _pingMonitoringJobManager;

    public PingingController(PingMonitoringJobManager pingMonitoringJobManager, ILogger<PingingController> logger)
    {
        _logger = logger;
        _pingMonitoringJobManager = pingMonitoringJobManager;
    }
    

    public enum OperationType
    {
        SUM,
        MULTIPLY
    };

    /*
    public record Operation(int number1, int number2);
    [HttpPost]
    [Route("/")]
    public int Sum([FromHeader] OperationType operationType, [FromBody] Operation operation)
    {
        return (operationType.ToString() == "SUM") ? operation.number1 + operation.number2
                                                   : operation.number1 * operation.number2;
    }

    [HttpGet]
    [Route("/{number1}/{number2}/sum")]
    public int Sum(int number1, int number2)
    {
        return number1 + number2;
    }
    
    
    [HttpPut]
    [Route("/status")]
    public string Get([FromQuery] string? x = null) => "asdf";
    */
    
    [HttpPut("StartPinging/{target}", Name = "StartPinging")]
    public IActionResult StartPinging(string target)
    {
        try
        {
            _logger.LogDebug($"PingingController: /StartPinging/{target}");
            _pingMonitoringJobManager.StartPinging(target);
            return Ok();
        }

        catch (Exception e)
        {
            Console.WriteLine($"{e}");
            return BadRequest();
        }
    }

    [HttpPost("StopPinging", Name = "StopPinging")]
    public IActionResult StopPinging()
    {
        _logger.LogDebug($"PingingController: /StopPinging");
        _pingMonitoringJobManager.StopPinging();
        return Ok();
    }
    

}