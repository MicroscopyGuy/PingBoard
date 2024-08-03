namespace PingBoard.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingBoard.Database.Utilities;
using PingBoard.Database.Models;

[ApiController]
[Route("DatabaseController")]
public class DatabaseController : ControllerBase
{
    private static ILogger<DatabaseController> _logger;
    private IDbContextFactory<PingInfoContext> _pingInfoContextFactory;
    
    public DatabaseController(IDbContextFactory<PingInfoContext> pingInfoContextFactory,
        ILogger<DatabaseController> logger)
    {
        _pingInfoContextFactory = pingInfoContextFactory;
        _logger = logger;
    }
    
    //[HttpGet("GetAnomalies/{StartingTime}/{NumberToRetrieve}", Name = "GetAnomalies")]
    //public IActionResult GetLastNAnomalies(DateTime StartingTime)
}
