using System.Text;
using Microsoft.Extensions.Options;
using PingBoard.Pinging.Configuration;

namespace PingBoard.Pinging{

    /// <summary>
    /// Provides a set of enums and several utilities for:
    ///     A) calculating if a PingGroup has tripped the set thresholds,
    ///     B) setting the apropriate flags to mark which thresholds have been tripped
    /// and C) outputing as a message, which thresholds were exceeded    
    /// </summary>
    public class PingQualification{
        
        /// <summary>
        /// Contains the values for thresholds set in the appsettings.json config file
        /// Needed in order to set the ThresholdFlags.
        /// </summary>
        private readonly PingingThresholdsConfig _pingThresholds;

        /// <summary>
        /// Flags used to encode and decode qualitative information about a ping group,
        /// namely High: minimum ping, average ping, maximum ping, jitter and packet loss. 
        /// And of course, if the thresholds were "NotExceeded."
        /// </summary>
        [Flags]
        public enum ThresholdExceededFlags : byte{
            NotExceeded     = (byte) 0,
            HighMinimumPing = (byte) 1 << 0,
            HighAveragePing = (byte) 1 << 1,
            HighMaximumPing = (byte) 1 << 2,
            HighJitter      = (byte) 1 << 3,
            HighPacketLoss  = (byte) 1 << 4,
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
        /// 
        public ThresholdExceededFlags CalculatePingQualityFlags(PingGroupSummary pingGroupInfo){
            ThresholdExceededFlags pingQualityFlags = pingGroupInfo.PingQualityFlags;

            if (pingGroupInfo.MinimumPing > _pingThresholds.MinimumPingMs) pingQualityFlags |= ThresholdExceededFlags.HighMinimumPing;
            if (pingGroupInfo.AveragePing > _pingThresholds.AveragePingMs) pingQualityFlags |= ThresholdExceededFlags.HighAveragePing;
            if (pingGroupInfo.MaximumPing > _pingThresholds.MaximumPingMs) pingQualityFlags |= ThresholdExceededFlags.HighMaximumPing;
            if (pingGroupInfo.Jitter > _pingThresholds.JitterMs)           pingQualityFlags |= ThresholdExceededFlags.HighJitter;
            if (pingGroupInfo.PacketLoss > _pingThresholds.PacketLossPercentage) pingQualityFlags |= ThresholdExceededFlags.HighPacketLoss;

            return pingQualityFlags;
        }

        /// <summary>
        /// Outputs a string indicating which of the thresholds were exceeded
        /// </summary>
        /// <param name="pingQualityFlags"></param>
        /// <returns></returns>
        public static string DescribePingQualityFlags(ThresholdExceededFlags pingQualityFlags){
            // the overwhelming majority (on average, >99%) of pings should have no flags set, 
            // so worth a quick check before any iteration
            if (PingQualification.PingQualityWithinThresholds(pingQualityFlags)){
                return "";
            }
            
            StringBuilder qualityDescription = new StringBuilder();
            foreach (ThresholdExceededFlags flag in Enum.GetValues(typeof(ThresholdExceededFlags))){
                if (flag == ThresholdExceededFlags.NotExceeded){
                    continue;
                }

                if ((pingQualityFlags & flag) == flag){
                    if (qualityDescription.Length > 0){
                        qualityDescription.Append(", ");
                    }
                    qualityDescription.Append(flag.ToString());
                }
            }
            return qualityDescription.ToString();
        }

        /// <summary>
        /// Quick and easy function to detect if all the flags are clear
        /// </summary>
        /// <param name="pingQualityFlags"></param>
        /// <returns></returns>
        public static bool PingQualityWithinThresholds(ThresholdExceededFlags pingQualityFlags){
            return pingQualityFlags == ThresholdExceededFlags.NotExceeded;
        }

    }
}