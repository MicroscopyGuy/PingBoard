using Microsoft.Extensions.Options;
using FluentValidation;

namespace PingBoard.Pinging{
    public class PingingBehaviorConfigValidator : AbstractValidator<PingingBehaviorConfig>{
        public PingingBehaviorConfigValidator(){
            RuleFor(pbc => pbc.PayloadStr).Length(1, 64);
            RuleFor(pbc => pbc.PingsPerCall).InclusiveBetween(1, 32);
            RuleFor(pbc => pbc.TimeoutMs).InclusiveBetween(500, 3500);
            RuleFor(pbc => pbc.Ttl).InclusiveBetween(1, 255);
            RuleFor(pbc => pbc.WaitMs).InclusiveBetween(1000, 3600000);
        }
    }
}



