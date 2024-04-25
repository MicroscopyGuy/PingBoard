using System.Net;

namespace PingBoard.Pinging{
    public struct PingGroupSummary{
        public DateTime? TimeSendAttempt { get; set;}
        public DateTime? TimeReceived {get; set;} 
        public string? Description{get; set;} 
        public string? ErrorDescription {get; set;}
        public string? Target {get; set;}
        public float? MinimumPing  {get; set;} 
        public float? AveragePing {get; set;} 
        public float? MaximumPing {get; set;}
        public float? Jitter {get; set;}
        public float? PacketLoss {get; set;}
        public IPAddress? ResolvedTarget {get; set;
        
        // consider adding a state summary here, details to be determined later
        }


        public static PingGroupSummary Empty(){
            return new PingGroupSummary(){
                TimeSendAttempt = DateTime.MinValue,
                TimeReceived    = DateTime.MaxValue,
                MinimumPing     = float.MaxValue,
                MaximumPing     = float.MinValue,
                AveragePing     = 0
            };
        }
    }
}