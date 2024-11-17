namespace PingBoard.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using Google.Protobuf.WellKnownTypes;
using PingBoard.Pinging;


/// <summary>
/// Defines a class meant to encapsulate the values returned by the SendPingGroupAsync() function
/// in the <see cref="GroupPinger"/> class.
/// </summary>
public class PingResult{
        
    /// <summary>
    /// The time the attempt to send the group of pings either started, or attempted to start
    /// </summary>
    public DateTime Start { get; set;}

    /// <summary>
    /// The time the attempt to receive the group of pings ended
    /// </summary>
    public DateTime End {get; set;} 
    
    /// <summary>
    /// The return time of the ping
    /// <summary>
    public int Rtt { get; set; }

    /// <summary>
    /// Wherever the user said to ping, could be either a domain or an IP address
    /// </summary>
    public string Target {get; set;}
    
    /// <summary>
    /// NEW: Whether the user-specified address is Ipv4 or Ipv6
    /// </summary>
    public string TargetType { get; set; }
    
    /// <summary>
    /// Indicates which IPStatus was returned
    /// </summary>
    public IPStatus? IpStatus {get; set;}
    
    /// <summary>
    /// The ttl of the ping request
    /// </summary>
    public short Ttl { get; set; }

    /// <summary>
    /// The IP address of the machine which sent the reply
    /// </summary>
    public string ReplyAddress { get; set; }
    

    /// <summary>
    /// Safely initializes and returns a PingGroupSummmary object with five properties safely intialized to default values:
    ///     Start, End, MinimumPing, MaximumPing and AveragePing
    /// </summary>
    /// <returns>
    ///     a PingGroupSummary object 
    /// </returns>
    public static PingResult Empty(){
        return new PingResult(){
            Start = DateTime.MinValue,
            End   = DateTime.MaxValue,
            IpStatus = null,
            PingQualityFlags = 0b0000_0000 // bitmask for PingQualityFlags
        };
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"Rtt: {Rtt} IpStatus: {IpStatus.ToString()} " + $"EndTime: {End.ToString("MM:dd:yyyy:hh:mm:ss.ffff")}" +
               $"Target: {Target} ReplyAddress: {ReplyAddress}";
    }
    
    public static PingResultPublic ToApiModel(PingResult result)
    {
        return new PingResultPublic
        {
            Start = Timestamp.FromDateTime(DateTime.SpecifyKind(result.Start, DateTimeKind.Utc)),
            End = Timestamp.FromDateTime(DateTime.SpecifyKind(result.End, DateTimeKind.Utc)),
            Target = result.Target,
            TargetType = result.TargetType,
            IpStatus = result.IpStatus.ToString(),
            Ttl = result.Ttl,
            ReplyAddress = result.ReplyAddress
        };
    }
    
    public static implicit operator PingResultPublic(PingResult result)
        => PingResult.ToApiModel(result);
    
    

}
