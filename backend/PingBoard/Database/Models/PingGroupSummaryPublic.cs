namespace PingBoard.Database.Models;

/// <summary>
/// The model used to display PingGroupSummary information to the user after it's retrieved from the database.
/// 
/// </summary>
public class PingGroupSummaryPublic
{

    /// <summary>
    /// The time the attempt to send the group of pings either started, or attempted to start
    /// </summary>
    public DateTime Start { get; set; }

    /// <summary>
    /// The time the attempt to receive the group of pings ended
    /// </summary>
    public DateTime End { get; set; }

    /// <summary>
    /// Wherever the user said to ping, could be either a domain or an IP address
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// The lowest of the recorded pings in the group
    /// </summary>
    public short MinimumPing { get; set; }

    /// <summary>
    /// The average measurement of all the pings in the group
    /// </summary>
    public float AveragePing { get; set; }

    /// <summary>
    /// The highest of the recorded pings in the group
    /// </summary>         
    public short MaximumPing { get; set; }

    /// <summary>
    /// The sum of the adjacent differences in ping time for all pings in the group
    /// </summary>
    public float Jitter { get; set; }

    /// <summary>
    /// The percentage of packets that were sent and not received, ie, the resulted in a returned TimedOut IPStatus 
    /// </summary>
    public float PacketLoss { get; set; }

    /// <summary>
    /// Safely initializes and returns a PingGroupSummmary object with five properties safely intialized to default values:
    ///     Start, End, MinimumPing, MaximumPing and AveragePing
    /// </summary>
    /// <returns>
    ///     a PingGroupSummary object 
    /// </returns>
    public static PingGroupSummaryPublic Empty()
    {
        return new PingGroupSummaryPublic()
        {
            Start = DateTime.MinValue,
            End = DateTime.MaxValue,
            Target = "",
            MinimumPing = short.MaxValue,
            MaximumPing = short.MinValue,
            AveragePing = 0,
            PacketLoss = 0F,
            Jitter = 0F
        };
    }

    public static PingGroupSummaryPublic Parse(PingGroupSummary summary)
    {
        return new PingGroupSummaryPublic
        {
            Start = summary.Start,
            End = summary.End,
            Target = summary.Target,
            MinimumPing = summary.MinimumPing,
            AveragePing = summary.AveragePing,
            MaximumPing = summary.MaximumPing,
            PacketLoss = summary.PacketLoss,
            Jitter = summary.Jitter
        };
    }
    
    public static implicit operator PingGroupSummaryPublic(PingGroupSummary summary)
        => PingGroupSummaryPublic.Parse(summary);

}