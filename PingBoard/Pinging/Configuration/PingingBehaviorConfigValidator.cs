using Microsoft.Extensions.Options;
using FluentValidation;
using PingBoard.Pinging.Configuration;
using Limits = PingBoard.Pinging.Configuration.PingingBehaviorConfigLimits;

namespace PingBoard.Pinging.Configuration{
    public class PingingBehaviorConfigValidator : AbstractValidator<PingingBehaviorConfig>{
        public PingingBehaviorConfigValidator(PingingBehaviorConfigLimits pingingBehaviorLimits){
            RuleFor(pbc => pbc.PayloadStr)
                .Length(Limits.ShortestAllowedPayloadStr, Limits.LongestAllowedPayloadStr);

            RuleFor(pbc => pbc.PingsPerCall)
                .InclusiveBetween(Limits.FewestAllowedPingsPerCall, Limits.MostAllowedPingsPerCall);

            RuleFor(pbc => pbc.TimeoutMs)
                .InclusiveBetween(Limits.ShortestAllowedTimeoutMs, Limits.LongestAllowedTimeoutMs);

            RuleFor(pbc => pbc.Ttl)
                .InclusiveBetween(Limits.ShortestAllowedTtl, Limits.LongestAllowedTtl);

            RuleFor(pbc => pbc.WaitMs)
                .InclusiveBetween(Limits.ShortestAllowedWaitMs, Limits.LongestAllowedWaitMs);

            RuleFor(pbc => pbc.ReportBackAfterConsecutiveTimeouts)
                .InclusiveBetween(pingingBehaviorLimits.FewestAllowedConsecutiveTimeoutsBeforeReportBack, pingingBehaviorLimits.MostAllowedConsecutiveTimeoutsBeforeReportBack);
                /*.WithMessage($"""
                                The ReportBackAfterTimeouts property can't be less than FewestPingsPerCall, or more than than MostPingsPerCall.
                                Additionally it is *strongly* encouraged to leave this value at 2, as it increases the responsiveness of 
                                the program's outage reporting. It is currently configured to {} and 
                             """);*/
                
        }
    }
}



