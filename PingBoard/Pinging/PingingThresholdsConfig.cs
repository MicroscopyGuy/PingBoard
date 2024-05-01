namespace PingBoard.Pinging{
    /// <summary>
    /// A simple class for storing desired thresholds for pinging. Said differently,
    /// these are the thresholds- beyond which a ping group is considered to be "problematic"
    /// </summary>
    public class PingingThresholdsConfig{

        
        /// <summary>
        /// A ping group's minimum ping > MinimumPingMs is said to be problematic
        /// </summary>
        public short MinimumPingMs {get; set;}

        /// <summary>
        /// A ping group's avg ping > AveragePingMs is said to be problematic
        /// </summary>
        public float AveragePingMs {get; set;}

        /// <summary>
        /// A ping group's maximum ping > MaximumPingMs is said to be problematic
        /// </summary>
        public short MaximumPingMs {get; set;}

        /// <summary>
        /// A ping group's jitter > JitterMs is said to be problematic
        /// </summary>
        public float JitterMs {get; set;}

        /// <summary>
        /// A ping group's % of loss packets > than PacketLossPercentage is said to be problematic
        /// </summary>
        public float PacketLossPercentage {get; set;}
    }
}