namespace PingBoard.Pinging.Configuration{
    /// <summary>
    /// A simple class for storing desired thresholds for pinging. Said differently,
    /// these are the thresholds- beyond which a ping group is considered to be "problematic"
    /// </summary>
    public class PingingThresholdsConfig{

        /// <summary>
        /// A ping group's minimum ping > MinimumPingMs is said to be problematic
        /// </summary>
        public int MinimumPingMs {get; set;}

        /// <summary>
        /// A ping group's avg ping > AveragePingMs is said to be problematic
        /// </summary>
        public float AveragePingMs {get; set;}

        /// <summary>
        /// A ping group's maximum ping > MaximumPingMs is said to be problematic
        /// </summary>
        public int MaximumPingMs {get; set;}

        /// <summary>
        /// A ping group's jitter > JitterMs is said to be problematic
        /// </summary>
        public float JitterMs {get; set;}

        /// <summary>
        /// A ping group's % of loss packets > than PacketLossPercentage is said to be problematic
        /// </summary>
        public int PacketLossPercentage {get; set;}

        /// <summary>
        /// The number of consecutively reported packet timeouts required for the GroupPinger
        /// to stop pinging (if not on the last ping) and report back that there may be an 
        /// outage of some kind.
        /// 
        /// A separate threshold in the MonitoringThresholds section of appsettings.json
        /// allows allows a user to configure the number of these reports required
        /// for the outage to be confirmed.
        /// 
        /// Please note that altering either the PossibleOutageAfterTimeouts,
        /// or OutageAfterReports properties in appsettings.json is strongly discouraged,
        /// and limited to 2 in PingingThresholdsLimits.cs, and MonitoringThresholdsLimits.cs.
        /// 
        /// Increasing either PossibleOutageAfterTimeouts or OutageAfterReports will drastically
        /// increase the time it takes the application to report an outage. Additionally note that
        /// historical ping analysis will not reflect a change in these values, as to save
        /// on memory and storage space, the results of all pings are intentionally not kept -- 
        /// only a group's summary (PingGroupSummary).Note that neither performance nor functionality 
        /// guarantees can be made in the event these figures are changed, 
        ///                     *the user will do so at their own peril.*
        /// 
        /// If a user *still* wishes to increase these thresholds, they may do so only by altering the source
        /// code directly. First alter the limits in PingingThresholdsLimits.cs and MonitoringThresholdsLimits.cs,
        /// and only then configure different values in appsettings.json.
        /// </summary>
        public int PossibleOutageAfterTimeouts {get; set;}

    }
}