using PingBoard.Services;

namespace PingBoard.Probes.NetworkProbes;

public class ProbeResult
{
    public Guid Id;
    public bool Success;
    public Exception Error;

    public static ProbeResult Default()
    {
        return new ProbeResult() { Success = true };
    }
}
