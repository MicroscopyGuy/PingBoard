namespace PingBoard.Services;

public class ProbeResult
{
    public Guid Id;
    public bool Success;
    public Exception Error;
    public ProbeStrategy Strategy;
    
    public static ProbeResult Default()
    {
        return new ProbeResult()
        {
            Success = true
        };
    }
}