namespace PingBoard.Monitoring;
using Microsoft.Extensions.Options;
using PingBoard.Monitoring.Configuration;

public class OutageSentinel{

    public int consecutiveTimeouts {get; set;}

    public OutageSentinel(IOptions<MonitoringBehaviorConfig> monitoringBehavior){
        consecutiveTimeouts = 0;
    }

    public void ResetTimeouts(){
        
    }
}