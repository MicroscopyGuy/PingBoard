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
    
    
    [HttpPut("StartPinging/{target}", Name = "StartPinging")]
    public IActionResult StartPinging(string target)
    {
        try
        {
            if (_pingMonitoringJobManager.IsPinging())
            {
                return StatusCode(409);
            }
            
            _logger.LogDebug($"PingingController: /StartPinging/{target}");
            _pingMonitoringJobManager.StartPinging(target);
            //return Ok();
            return StatusCode(204);
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