namespace PingBoard.Pinging.Configuration{
    /// <summary>
    /// Stores limits used by PingingThresholdsConfigValidator to validate the PingingThresholds-specific
    /// information configured in appsettings.json
    /// </summary>
    public class PingingThresholdsConfigLimits{
        /// <summary>
        /// The lowest value allowed for the MinimumPing threshold, measured in milliseconds
        /// </summary>
        public const int LowestAllowedMinimumPingMs = 10;
        
        /// <summary>
        /// The lowest value allowed for the AveragePing threshold, measured in milliseconds
        /// </summary>
        public const float LowestAllowedAveragePingMs = 11;
        
        /// <summary>
        /// The lowest value allowed for the MaximumPing threshold, measured in milliseconds
        /// </summary>
        public const int LowestAllowedMaximumPingMs = 12;

        /// <summary>
        /// The lowest value allowed for the Jitter threshold, measured in milliseconds
        /// </summary>
        public float LowestAllowedJitterThresholdMs = LowestAllowedAveragePingMs/2; 

        /// <summary>
        /// The lowest value allowed for the PacketLossPercentage threshold
        /// </summary>
        public const int LowestAllowedPacketLossPercentage = 0;

        /// <summary>
        /// The highest value allowed for the MinimumPing threshold, measured in milliseconds
        /// </summary>
        public const int HighestAllowedMinimumPingMs = 95;

        /// <summary>
        /// The lowest value allowed for the AveragePing threshold, measured in milliseconds
        /// </summary>
        public const float HighestAllowedAveragePingMs = 100;

        /// <summary>
        /// The lowest value allowed for the MaximumPing threshold, measured in milliseconds
        /// </summary>
        public const int HighestAllowedMaximumPingMs = 200;

        /// <summary>
        /// The lowest value allowed for the Jitter threshold, measured in milliseconds
        /// </summary>
        public float HighestAllowedJitterThresholdMs = HighestAllowedAveragePingMs/2;

        /// <summary>
        /// The highest value allowed for the PacketLossPercentage threshold
        /// </summary>
        public const int HighestAllowedPacketLossPercentage = 100;

    }
}