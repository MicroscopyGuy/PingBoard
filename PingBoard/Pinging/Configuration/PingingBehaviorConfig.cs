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
    }
}