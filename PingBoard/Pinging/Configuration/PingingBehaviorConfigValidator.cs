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
        }
    }
}



