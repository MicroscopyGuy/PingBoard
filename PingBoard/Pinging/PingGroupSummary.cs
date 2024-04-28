using System.Net;
using System.Net.NetworkInformation;

namespace PingBoard.Pinging{
    public struct PingGroupSummary{

        /// <summary>
        /// The time the attempt to send the group of pings either started, or attempted to start
        /// </summary>
        public DateTime? Start { get; set;}

        /// <summary>
        /// The time the attempt to receive the group of pings ended
        /// </summary>
        public DateTime? End {get; set;} 
        
        /// <summary>
        /// If an exception occurred during the SendPingAsync function call, this will indicate which
        /// </summary>
        public string? ExceptionDescription {get; set;}
        
        /// <summary>
        /// Wherever the user said to ping, could be either a domain or an IP address
        /// </summary>
        public string? Target {get; set;}

        /// <summary>
        /// The lowest of the recorded pings in the group
        /// </summary>
        public float? MinimumPing  {get; set;} 
        
        /// <summary>
        /// The average measurement of all the pings in the group
        /// </summary>
        public float? AveragePing {get; set;}

        /// <summary>
        /// The highest of the recorded pings in the group
        /// </summary>         
        public float? MaximumPing {get; set;}
        
        /// <summary>
        /// The variance of the pings in the group; how far apart from the average ping measurement the group was
        /// </summary>
        public float? Jitter {get; set;}
        /// <summary>
        /// The percentage of packets that were sent and not received, ie, the resulted in a returned TimedOut IPStatus 
        /// </summary>
        public float? PacketLoss {get; set;}

        /// <summary>
        /// If an IP status is returned that is mapped to the Halt state (in ICMPStatusCodes.json),
        /// this property will indicate which exact IPStatus was returned
        /// </summary>
        public IPStatus HaltingIPStatus {get; set;}

        /// <summary>
        /// Safely initializes and returns a PingGroupSummmary object with five properties safely intialized to default values:
        ///     Start, End, MinimumPing, MaximumPing and AveragePing
        /// </summary>
        /// <returns>
        ///     a PingGroupSummary object 
        /// </returns>
        public static PingGroupSummary Empty(){
            return new PingGroupSummary(){
                Start = DateTime.MinValue,
                End    = DateTime.MaxValue,
                MinimumPing     = float.MaxValue,
                MaximumPing     = float.MinValue,
                AveragePing     = 0
            };
        }
    }
}