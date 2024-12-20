namespace PingBoard.Database.Models;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using Google.Protobuf.WellKnownTypes;
using PingBoard.Pinging;
using Protos;

/// <summary>
/// Defines a class meant to encapsulate the values returned by the SendPingGroupAsync() function
/// in the <see cref="GroupPinger"/> class.
/// </summary>
public class PingGroupSummary
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
    /// The percentage of packets that were sent and not received, ie, that resulted in a returned TimedOut IPStatus
    /// </summary>
    public float PacketLoss { get; set; }

    /// <summary>
    /// If an IP status is returned that is mapped to the Halt state (in ICMPStatusCodes.json),
    /// this property will indicate which exact IPStatus was returned
    /// </summary>
    public IPStatus? TerminatingIPStatus { get; set; }

    /// <summary>
    /// Indicates the last received IPStatus which was not "Success".
    /// </summary>
    public IPStatus? LastAbnormalStatus { get; set; }

    /// <summary>
    /// The number of consecutive timeouts reported by GroupPinger
    /// </summary>
    public byte ConsecutiveTimeouts { get; set; }

    /// <summary>
    /// The number of packets sent by GroupPinger in one SendPingGroupAsync function call
    /// </summary>
    public byte PacketsSent { get; set; }

    /// <summary>
    /// The number of packets lost in one SendPingGroupAsync function call
    /// </summary>
    public byte PacketsLost { get; set; }

    /// <summary>
    /// The number of pings which cannot be used for calculating MinimumPing, AveragePing, MaximumPing or Jitter,
    /// because they reflect pings which did not return "IPStatus.Success"
    ///
    /// Critically, this information is NOT to be included in PacketLoss calculations. Packetloss refers to a specific
    /// scenario in which a packet is *lost*. Any other response is a valid, albeit possibly *unsuccessful* ping.
    /// </summary>
    public byte ExcludedPings { get; set; }

    /// <summary>
    /// Treated as a bitmap to compactly store information about the quality of the pings summarized by a PingGroupSummary.
    /// For more information, see ThresholdExceededFlags.cs.
    /// </summary>
    public PingGroupQualifier.ThresholdExceededFlags PingQualityFlags { get; set; }

    /// <summary>
    /// Safely initializes and returns a PingGroupSummmary object with five properties safely intialized to default values:
    ///     Start, End, MinimumPing, MaximumPing and AveragePing
    /// </summary>
    /// <returns>
    ///     a PingGroupSummary object
    /// </returns>
    public static PingGroupSummary Empty()
    {
        return new PingGroupSummary()
        {
            Start = DateTime.MinValue,
            End = DateTime.MaxValue,
            MinimumPing = short.MaxValue,
            MaximumPing = short.MinValue,
            AveragePing = 0,
            PacketsLost = 0,
            PacketsSent = 0,
            PacketLoss = 0F,
            ConsecutiveTimeouts = 0,
            ExcludedPings = 0,
            PingQualityFlags = 0b0000_0000, // bitmask for PingQualityFlags
        };
    }

    /// <summary>
    ///     Calculates and returns the standard deviation of a List of ping times
    ///     Presently unused, and untested.
    /// </summary>
    /// <param name="responseTimes">A list of response times to be analyzed</param>
    /// <param name="mean">The average of the ping times stored in responseTimes</param>
    /// <returns>The standard deviation of the ping times in response times</returns>
    [ExcludeFromCodeCoverage]
    public static float CalculatePingStdDeviation(List<long> responseTimes, float mean)
    {
        if (responseTimes.Count <= 1)
        {
            return 0;
        }

        float sumSquaredMeanDiff = 0;
        foreach (long rtt in responseTimes)
        {
            sumSquaredMeanDiff += (float)Math.Pow(rtt - mean, 2);
        }

        return (float)Math.Sqrt(sumSquaredMeanDiff / responseTimes.Count);
    }

    /// <summary>
    /// Calculates and returns the jitter (sum of adjacent differences) of a List of ping times
    /// </summary>
    /// <param name="responseTimes">A List of ping times</param>
    /// <returns>The calculated jitter of the List of pings</returns>
    public static float CalculatePingJitter(List<long> responseTimes)
    {
        if (responseTimes.Count <= 1)
        {
            return 0;
        }

        double jitter = 0;
        for (int i = 0; i < responseTimes.Count - 1; i++)
        {
            jitter += Math.Abs(responseTimes[i] - responseTimes[i + 1]);
        }

        jitter /= responseTimes.Count - 1;
        return (float)Math.Round(jitter, 3);
    }

    /// <summary>
    /// Calculates and returns the average of a set of ping times, taking into account
    /// those that were lost.
    /// </summary>
    /// <param name="info">
    ///         PingGroupSummary object which contains the cumulative ping sum so far, as well as information
    ///         on how many pings should be excluded from the Avg calculation (excluded = lost + excluded)
    /// </param>
    /// <returns></returns>
    public static float CalculateAveragePing(PingGroupSummary info)
    {
        //ExcludedPings also contains any pings that were lost.
        int numPingsToAverage = info.PacketsSent - info.ExcludedPings;
        if (numPingsToAverage == 0)
        {
            return 0;
        }

        return (float)Math.Round(info.AveragePing / numPingsToAverage, 3);
    }

    /// <summary>
    /// Calculates the % of packets lost from a set of attempted pings
    /// </summary>
    /// <param name="packetsSent"> The number of packets sent</param>
    /// <param name="packetsLost"> The number of packets lost</param>
    /// <returns>The calculated packet loss</returns>
    public static float CalculatePacketLoss(int packetsSent, int packetsLost)
    {
        if (packetsLost == 0)
        {
            return 0;
        }

        float frac = (float)packetsLost / packetsSent;
        return 100 * (float)Math.Round(frac, 3);
    }

    /// <summary>
    ///    Sets a new "MaximumPing" property on a PingGroupSummary object
    ///    if the provided return time is greater than the one currently stored
    /// </summary>
    /// <param name="summary"> The PingGroupSummary object being worked on</param>
    /// <param name="rtt"> A new ping time to be tested</param>
    public static void SetIfMaxPing(PingGroupSummary summary, short rtt)
    {
        if (rtt > summary.MaximumPing)
        {
            summary.MaximumPing = rtt;
        }
    }

    /// <summary>
    ///    Sets a new "MaximumPing" property on a PingGroupSummary object
    ///    if the provided return time is less than the one currently stored
    /// </summary>
    /// <param name="summary"> The PingGroupSummary object being worked on</param>
    /// <param name="rtt"> A new ping time to be tested</param>
    public static void SetIfMinPing(PingGroupSummary summary, short rtt)
    {
        if (rtt < summary.MinimumPing)
        {
            summary.MinimumPing = rtt;
        }
    }

    /// <summary>
    /// Prevents initialized short.MaxValue, short.MinValue values for Min/Max pings, respectively,
    /// from persisting after a SendPingGroupAsync function call. Useful in cases where
    /// none of the pings in the group returned "IPStatus.Success"
    /// </summary>
    /// <param name="summary">The PingGroupSummary object being worked on</param>
    public static void ResetMinMaxPingsIfUnused(PingGroupSummary summary)
    {
        if (summary is not { MinimumPing: short.MaxValue, MaximumPing: short.MinValue })
            return;
        summary.MinimumPing = 0;
        summary.MaximumPing = 0;
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"MinimumPing: {MinimumPing} AveragePing: {AveragePing} "
            + $"MaximumPing: {MaximumPing} Jitter: {Jitter} PacketLoss: {PacketLoss} "
            + $"TerminatingIPStatus: {TerminatingIPStatus} EndTime: {End.ToString("MM:dd:yyyy:hh:mm:ss.ffff")}";
    }

    public static PingGroupSummaryPublic ToApiModel(PingGroupSummary summary)
    {
        return new PingGroupSummaryPublic
        {
            Start = Timestamp.FromDateTime(DateTime.SpecifyKind(summary.Start, DateTimeKind.Utc)),
            End = Timestamp.FromDateTime(DateTime.SpecifyKind(summary.End, DateTimeKind.Utc)),
            Target = summary.Target,
            MinimumPing = summary.MinimumPing,
            AveragePing = summary.AveragePing,
            MaximumPing = summary.MaximumPing,
            PacketLoss = summary.PacketLoss,
            Jitter = summary.Jitter,
        };
    }

    public static implicit operator PingGroupSummaryPublic(PingGroupSummary summary) =>
        PingGroupSummary.ToApiModel(summary);
}
