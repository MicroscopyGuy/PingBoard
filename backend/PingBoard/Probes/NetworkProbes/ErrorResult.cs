namespace PingBoard.Probes.NetworkProbes;

public record ErrorResult(Exception Ex)
{
    private readonly Exception _ex = Ex;

    public Exception Ex
    {
        get => _ex;
        init => _ex = value;
    }
}
