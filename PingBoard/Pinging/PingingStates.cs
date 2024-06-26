
namespace PingBoard.Pinging{
    public static class PingingStates{

        /// <summary>
        ///  Provides a set of three Ping States which determine whether, and how the next *individual* ping should proceed
        /// </summary>
        public enum PingState{
            Continue,         // Just keep going
            Pause,            // requires waiting for something to resolve, like a SourceQuench ICMP error
            Halt,             // fatal issue which requires change of program input
            PacketLossCaution // Packet loss detected, update some 
        }
    }
}