namespace PingBoard.Pinging{
    /// <summary>
    /// A simple class for storing desired thresholds for pinging. Said differently,
    /// these are the thresholds- beyond which a ping group is considered to be "problematic"
    /// </summary>
    public class PingingThresholdsConfig{
        /// <summary>
        /// A ping group's avg ping > AveragePingMs is said to be problematic
        /// </summary>
        public int AveragePingMs {get; set;}

        /// <summary>
        /// A ping group's jitter > JitterMs is said to be problematic
        /// </summary>
        public int JitterMs {get; set;}

        /// <summary>
        /// A ping group's % of loss packets > than PacketLossPercentage is said to be problematic
        /// </summary>
        public float PacketLossPercentage {get; set;}
    }
}