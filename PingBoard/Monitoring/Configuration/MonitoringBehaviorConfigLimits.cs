namespace PingBoard.Monitoring.Configuration;
using PingingLimits = PingBoard.Pinging.Configuration.PingingBehaviorConfigLimits;

/// <summary>
/// 
/// </summary>
public class MonitoringBehaviorConfigLimits{
    /// <summary>
    /// The most timeouts allowed to be reported by GroupPinger before an outage is declared
    /// </summary>
    public const int MostAllowedConsecutiveTimeoutsBeforeOutage = 8;

    /// <summary>
    /// The fewest timeouts allowed to be reported by GroupPinger before an outage is declared
    /// </summary>
    public const int FewestAllowedConsecutiveTimeoutsBeforeOutage = 2;
}