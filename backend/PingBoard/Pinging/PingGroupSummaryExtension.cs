namespace PingBoard.Pinging;
using System.Diagnostics.CodeAnalysis;
using PingBoard.Database.Models;

public static class PingGroupSummaryExtension
{
    [ExcludeFromCodeCoverage]
    public static PingGroupSummary Empty(){
            return new PingGroupSummary(){
                Start = DateTime.MinValue,
                End   = DateTime.MaxValue,
                MinimumPing = short.MaxValue,
                MaximumPing = short.MinValue,
                AveragePing = 0,
                PacketsLost = 0,
                PacketsSent = 0,
                PacketLoss  = 0F,
                ConsecutiveTimeouts = 0,
                ExcludedPings = 0,
                PingQualityFlags = 0b0000_0000 // bitmask for PingQualityFlags
            };
        }
    
        /// <summary>
        ///     Calculates and returns the standard deviation of a List of ping times
        ///     Presently unused, and untested.
        /// </summary>
        /// <param name="responseTimes">A list of response times to be analyzed</param>
        /// <param name="mean">The average of the ping times stored in responseTimes</param>
        /// <returns>The standard deviation of the ping times in response times</returns>
        [ExcludeFromCodeCoverage]
        public static float CalculatePingStdDeviation(List<long> responseTimes, float mean){
            if (responseTimes.Count <= 1){ 
                return 0;
            }

            float sumSquaredMeanDiff = 0;
            foreach (long rtt in responseTimes){
                sumSquaredMeanDiff += (float) Math.Pow(rtt-mean, 2);
            }

            return (float) Math.Sqrt(sumSquaredMeanDiff / responseTimes.Count);
        }

        /// <summary>
        /// Calculates and returns the jitter (sum of adjacent differences) of a List of ping times
        /// </summary>
        /// <param name="responseTimes">A List of ping times</param>
        /// <returns>The calculated jitter of the List of pings</returns>
        public static float CalculatePingJitter(List<long> responseTimes){
            if (responseTimes.Count <= 1){
                return 0;
            }
            
            double jitter = 0;
            for (int i = 0; i < responseTimes.Count-1; i++){
                jitter += Math.Abs(responseTimes[i] - responseTimes[i+1]);
            }

            jitter /= responseTimes.Count-1;
            return (float) Math.Round(jitter, 3);
        }

        /// <summary>
        /// Calculates and returns the average of a set of ping times, taking into account
        /// those that were lost.
        /// </summary>
        /// <param name="info">
        ///         PingGroupSummary object which contains the cumulative ping sum so far, as well as information
        ///         on how many pings should be excluded from the Avg calculation (excluded = lost + excluded)
        /// </param>
        /// <returns></returns>
        public static float CalculateAveragePing(this PingGroupSummary info)
        {
            //ExcludedPings also contains any pings that were lost.
            int numPingsToAverage = info.PacketsSent - info.ExcludedPings;
            if (numPingsToAverage == 0) {
                return 0;
            }

            return (float) Math.Round(info.AveragePing / numPingsToAverage, 3);

        }

        /// <summary>
        /// Calculates the % of packets lost from a set of attempted pings
        /// </summary>
        /// <param name="packetsSent"> The number of packets sent</param>
        /// <param name="packetsLost"> The number of packets lost</param>
        /// <returns>The calculated packet loss</returns>
        public static float CalculatePacketLoss(int packetsSent, int packetsLost) {
            if (packetsLost == 0){
                return 0;
            }

            float frac = (float) packetsLost/packetsSent;
            return 100 * (float) Math.Round(frac, 3);
        }

        /// <summary>
        ///    Sets a new "MaximumPing" property on a PingGroupSummary object
        ///    if the provided return time is greater than the one currently stored
        /// </summary>
        /// <param name="summary"> The PingGroupSummary object being worked on</param>
        /// <param name="rtt"> A new ping time to be tested</param>
        public static void SetIfMaxPing(PingGroupSummary summary, short rtt){
            if (rtt > summary.MaximumPing){
                summary.MaximumPing = rtt;
            }
        }

        /// <summary>
        ///    Sets a new "MaximumPing" property on a PingGroupSummary object
        ///    if the provided return time is less than the one currently stored
        /// </summary>
        /// <param name="summary"> The PingGroupSummary object being worked on</param>
        /// <param name="rtt"> A new ping time to be tested</param>
        public static void SetIfMinPing(this PingGroupSummary summary, short rtt){
            if (rtt < summary.MinimumPing){
                summary.MinimumPing = rtt;
            }
        }

        /// <summary>
        /// Prevents initialized short.MaxValue, short.MinValue values for Min/Max pings, respectively,
        /// from persisting after a SendPingGroupAsync function call. Useful in cases where
        /// none of the pings in the group returned "IPStatus.Success"
        /// </summary>
        /// <param name="summary">The PingGroupSummary object being worked on</param>
        public static void ResetMinMaxPingsIfUnused(this PingGroupSummary summary) {
            if (summary is not { MinimumPing: short.MaxValue, MaximumPing: short.MinValue }) return;
            summary.MinimumPing = 0;
            summary.MaximumPing = 0;
        }

        [ExcludeFromCodeCoverage]
        public static string ToString(this PingGroupSummary summary)
        {
            return  $"MinimumPing: {summary.MinimumPing} AveragePing: {summary.AveragePing} " +
                    $"MaximumPing: {summary.MaximumPing} Jitter: {summary.Jitter} PacketLoss: {summary.PacketLoss} " +
                    $"TerminatingIPStatus: {summary.TerminatingIPStatus} EndTime: {summary.End:MM:dd:yyyy:hh:mm:ss.ffff}";
        }
}
