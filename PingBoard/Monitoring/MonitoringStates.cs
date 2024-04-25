
namespace PingBoard.Monitoring{
    public static class MonitoringStates{
        /// <summary>
        ///  Provides a set of four states which determines whether and how the next ping *group* should proceed
        /// </summary>
        public enum MonitorState{
            Continue,    // no issue, what we want,
            DoubleCheck, // needs confirmation of network issue reported by ping group
            Pause,       // requires waiting for something to resolve, like a SourceQuench ICMP error
            Halt         // fatal issue which requires change of program input
        }
    }
}