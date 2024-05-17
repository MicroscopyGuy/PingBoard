namespace PingBoard.Monitoring.Configuration;
using PingBoard.Pinging.Configuration;
using Microsoft.Extensions.Options;
using FluentValidation;
using MonitoringLimits = PingBoard.Monitoring.Configuration.MonitoringBehaviorConfigLimits;


public class MonitoringBehaviorConfigValidator : AbstractValidator<MonitoringBehaviorConfig>{
    public MonitoringBehaviorConfigValidator(PingingBehaviorConfig pingingBehaviorConfig){
        RuleFor(mbc => mbc.OutageAfterTimeouts)
            .InclusiveBetween(MonitoringLimits.FewestAllowedConsecutiveTimeoutsBeforeOutage, 
                              MonitoringLimits.MostAllowedConsecutiveTimeoutsBeforeOutage)
            .WithMessage("""
                            For the sake of program responsiveness to outages, the OutageAfterTimeout property
                            must be kept low. It must also be greater than or equal to the ReportBackAfterConsecutiveTimeouts
                            property configured in the Pinging section of appsettings.json.
                        """);
        RuleFor(mbc => mbc.OutageAfterTimeouts)
            .GreaterThanOrEqualTo(pingingBehaviorConfig.ReportBackAfterConsecutiveTimeouts)
            .WithMessage($"""
                            The program cannot declare an outage at a *lower* number of timeouts than the number configured
                            in appsettings.json for ReportBackAfterConsecutiveTimeouts (currently set to 
                            {pingingBehaviorConfig.ReportBackAfterConsecutiveTimeouts})
                          """);
    }
}


