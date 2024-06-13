using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace PingBoard.Controllers;

[ApiController]
//[Route("/PingingController")]
public class PingingController : ControllerBase
{
    private static ILogger<PingingController> _logger;

    public PingingController(ILogger<PingingController> logger)
    {
        _logger = logger;
    }







    public enum OperationType
    {
        SUM,
        MULTIPLY
    };

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
    
    /*
    [HttpPut(Name = "StartPinging")]
    [Route("Pinging/Start")]
    public IActionResult StartPinging([FromQuery] string target)
    {
        for (int i = 0; i < 20; i++)
        {
            Console.WriteLine("Pinging");
        }

        return Ok();
    }
    
    [HttpPut(Name = "StopPinging")]
    [Route("/Pinging/Stop")]
    public void StopPinging()
    {
        Console.WriteLine("Pinging stopped");
    }*/

}