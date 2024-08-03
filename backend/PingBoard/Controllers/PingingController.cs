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
    
}