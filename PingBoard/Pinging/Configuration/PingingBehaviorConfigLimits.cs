using System.Reflection.Metadata;

namespace PingBoard.Pinging.Configuration{
    /// <summary>
    /// Stores limits used by PingingBehaviorConfigValidato to validate the PingingBehavior-specific 
    /// information configured in appsettings.json.
    /// </summary>
    public class PingingBehaviorConfigLimits{
        /// <summary>
        /// Shortest length allowed for the string sent as the payload (data) in every ping
        /// </summary>
        public const int ShortestAllowedPayloadStr = 1;

        /// <summary>
        /// Longest length allowed for the string sent as the payload (data) in every ping
        /// </summary>
        public const int LongestAllowedPayloadStr = 64;

        /// <summary>
        /// Fewest pings allowed to be sent in every ping group sent by a GroupPinger object
        /// </summary>
        public const int FewestAllowedPingsPerCall = 2;

         /// <summary>
        /// Most pings allowed to be sent in every ping group sent by a GroupPinger object
        /// </summary>
        public const int MostAllowedPingsPerCall = 32;

        /// <summary>
        /// Shortest amount of time, measured in milliseconds, allowed to elapse before a packet is considered lost
        /// </summary>
        public const int ShortestAllowedTimeoutMs = 500;

        /// <summary>
        /// Longest amount of time, measured in milliseconds, allowed to elapse before a packet is considered lost
        /// </summary>
        public const int LongestAllowedTimeoutMs = 3500;

        /// <summary>
        /// Shortest Ttl allowed for each ping call 
        /// </summary>
        public const int ShortestAllowedTtl = 1;

        /// <summary>
        /// Longest allowed for each ping call
        /// </summary>
        public const int LongestAllowedTtl = 255;

        /// <summary>
        /// Shortest wait time, measured in milliseconds, allowed for a ping group
        /// </summary>
        public const int ShortestAllowedWaitMs = 1000;

        /// <summary>
        /// Longest wait time, measured in milliseconds, allowed for a ping group
        /// </summary>
        public const int LongestAllowedWaitMs = 3600000;

        /// <summary>
        /// The fewest number of consecutive timeouts allowed before GroupPinger reports back to the
        /// caller
        /// </summary>
        public int FewestAllowedConsecutiveTimeoutsBeforeReportBack = FewestAllowedPingsPerCall;
        
        /// <summary>
        /// The most number of consecutive timeouts allowed before GroupPinger reports back to the
        /// caller
        /// </summary>
        public int MostAllowedConsecutiveTimeoutsBeforeReportBack = MostAllowedPingsPerCall/4;

        

    }
}