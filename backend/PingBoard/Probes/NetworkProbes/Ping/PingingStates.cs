namespace PingBoard.Probes.NetworkProbes.Ping;

public static class PingingStates
{
    /// <summary>
    ///  Provides a set of three Ping States which determine whether, and how the next *individual* ping should proceed
    /// </summary>
    public enum PingState
    {
        Continue, // Just keep going
        Pause, // requires waiting for something to resolve, like a SourceQuench ICMP error
        Halt // fatal issue which may require a change of program input
        ,
    }
}
