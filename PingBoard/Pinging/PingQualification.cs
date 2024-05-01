using System.Text;
using Microsoft.Extensions.Options;

namespace PingBoard.Pinging{

    public class PingQualification{
        /// <summary>
        /// Flags used to encode and decode qualitative information about a ping group,
        /// namely High: minimum ping, average ping, maximum ping, jitter and packet loss. 
        /// And of course, if the thresholds were "NotExceeded."
        /// </summary>
        private readonly PingingThresholdsConfig _pingThresholds;

        [Flags]
        public enum ThresholdExceededFlags{
            NotExceeded     = (byte) 0,
            HighMinimumPing = (byte) 1 >> 0,
            HighAveragePing = (byte) 1 >> 1,
            HighMaximumPing = (byte) 1 >> 2,
            HighJitter      = (byte) 1 >> 3,
            HighPacketLoss  = (byte) 1 >> 4
        }

        public PingQualification(IOptions<PingingThresholdsConfig> pingThresholds){
            _pingThresholds = pingThresholds.Value;
        }

        /// <summary>
        /// Uses the thresholds stored in _pingThresholds as well as the flags stored in the enum ThresholdExeededFlags
        /// in order to compute the byte PingGroupSummary.PingQualitySummary. To be used later to check for exceeded thresholds.
        /// </summary>
        /// <param name="pingGroupInfo">The PingGroupSummary object being worked on</param>
        /// <returns></returns>
        public byte CalculatePingQualityFlags(PingGroupSummary pingGroupInfo){
            byte pingQualityFlags = (byte) (pingGroupInfo.PingQualityFlags
                | ((pingGroupInfo.MinimumPing > _pingThresholds.MinimumPingMs) ? (byte)ThresholdExceededFlags.HighMinimumPing : 0b0)
                | ((pingGroupInfo.AveragePing > _pingThresholds.AveragePingMs) ? (byte)ThresholdExceededFlags.HighAveragePing : 0b0)
                | ((pingGroupInfo.MaximumPing > _pingThresholds.MaximumPingMs) ? (byte)ThresholdExceededFlags.HighMaximumPing : 0b0) 
                | ((pingGroupInfo.Jitter      > _pingThresholds.JitterMs)      ? (byte)ThresholdExceededFlags.HighJitter : 0b0)
                | ((pingGroupInfo.PacketLoss  > _pingThresholds.PacketLossPercentage) ? (byte)ThresholdExceededFlags.HighPacketLoss : 0b0)
            );
            return pingQualityFlags;
        }

        /// <summary>
        /// Outputs a string indicating which of the thresholds were exceeded
        /// </summary>
        /// <param name="pingQualityFlags"></param>
        /// <returns></returns>
        public static string DescribePingQualityFlags(byte pingQualityFlags){
            // on average, the overwhelming majority (>99%) of pings should have no flags set, 
            // so worth a quick check before any iteration
            if (PingQualification.PingQualityWithinThresholds(pingQualityFlags)){
                return "";
            }
            
            StringBuilder qualityDescription = new StringBuilder();
            foreach (ThresholdExceededFlags flag in Enum.GetValues(typeof(ThresholdExceededFlags))){
                if ((pingQualityFlags & (byte) flag) == (byte) flag){
                    qualityDescription.Append(flag.ToString() + ", ");
                }
            }
            return qualityDescription.ToString();
        }

        /// <summary>
        /// Quick and easy function to detect if all the flags are clear
        /// </summary>
        /// <param name="pingQualityFlags"></param>
        /// <returns></returns>
        public static bool PingQualityWithinThresholds(byte pingQualityFlags){
            return pingQualityFlags == (byte) ThresholdExceededFlags.NotExceeded;
        }
    }
}