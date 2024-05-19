namespace PingBoard.Monitoring;
using Microsoft.Extensions.Options;
using PingBoard.Monitoring.Configuration;

public class OutageSentinel{

    public int consecutiveTimeouts {get; set;}
    private MonitoringBehaviorConfig _monitoringBehavior;

    public OutageSentinel(IOptions<MonitoringBehaviorConfig> monitoringBehavior){
        consecutiveTimeouts = 0;
        _monitoringBehavior = monitoringBehavior.Value;
    }

    public void ResetTimeouts(){
        
    }
}