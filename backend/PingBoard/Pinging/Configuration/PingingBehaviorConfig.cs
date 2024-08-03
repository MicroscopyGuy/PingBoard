namespace PingBoard.Pinging.Configuration{
    /// <summary>
    /// A simple class for holding associated with configurable pinging behavior
    /// </summary>
    public class PingingBehaviorConfig{

        /// <summary>
        /// The actual payload contained in the packets sent via each individual ping call
        /// </summary>
        public string? PayloadStr {get; set;}

        /// <summary>
        /// The number of pings to be sent in every group
        /// </summary>
        public int PingsPerCall {get; set;}

        /// <summary>
        /// Duration of time (measured in ms) before a ping is considered TimedOut (lost)
        /// </summary>
        public int TimeoutMs {get; set;}

        /// <summary>
        /// The TTL (time to live) set for the ping call, the PingSender class defaults to 128
        /// </summary>
        public int Ttl {get; set;} 

        /// <summary>
        /// Duration of time (measured in ms) to wait before sending another group of pings
        /// </summary>
        public int WaitMs {get; set;}

        /// <summary>
        /// The number of consecutively reported packet timeouts required for the GroupPinger
        /// to stop pinging (if not on the last ping) and report back that there may be an 
        /// outage of some kind.
        /// 
        /// A separate threshold in the MonitoringBehavior section of appsettings.json
        /// allows allows a user to configure the number of these reports required
        /// for the outage to be confirmed.
        /// 
        /// Please note that altering either the ReportBackAfterTimeouts or OutageAfterTimeouts,
        /// properties in appsettings.json is strongly discouraged, and limited in 
        /// PingingBehaviorConfigLimits, and MonitoringBehaviorConfigLimits.
        /// 
        /// Increasing either ReportBackAfterTimeouts or OutageAfterTimeouts will drastically
        /// increase the time it takes the application to report an outage. Additionally note that
        /// historical ping analysis *will not* reflect a change in these values, since only a ping 
        /// group's summary (PingGroupSummary) is retained, to same on memory and storage space.
        /// 
        /// WARNING: Note that neither performance nor functionality guarantees can be made in the event 
        ///          these limits are changed from their original values: 
        ///                           *the user will do so at their own peril.*
        /// 
        /// If a user *still* wishes to increase these properties, they may do so only by altering the source
        /// code directly. Instructions to do this are not provided.
        /// </summary>
        public int ReportBackAfterConsecutiveTimeouts {get; set;}
    }
}
