using Microsoft.Extensions.Options;
using FluentValidation;
using System.Reflection.Metadata;

namespace PingBoard.Pinging{
    /// <summary>
    /// Validates the values put in the PingingThresholds section of the appsettings.json file 
    /// </summary>
    public class PingingThresholdsConfigValidator : AbstractValidator<PingingThresholdsConfig>{

        private static readonly short _lowestAllowedMinimumPingMs = 10;
        private static readonly float _lowestAllowedAveragePingMs = 15;
        private static readonly short _lowestAllowedMaximumPingMs = 20;
        private static readonly float _lowestAllowedJitterThresholdMs = _lowestAllowedAveragePingMs/10;
        private static readonly float _lowestAllowedPacketLossPercentage = 0;
        private static readonly short _highestAllowedMinimumPingMs = 95;
        private static readonly float _highestAllowedAveragePingMs = 100;
        private static readonly short _highestAllowedMaximumPingMs = 200;
        private static readonly float _highestAllowedJitterThresholdMs = _highestAllowedAveragePingMs/2; 
        private static readonly float _highestAllowedPacketLossPercentage = 100;
        
        
        public PingingThresholdsConfigValidator(){
            RuleFor(ptc => ptc.MinimumPingMs)
                .LessThan(ptc => (short?) ptc.AveragePingMs)
                .InclusiveBetween(_lowestAllowedMinimumPingMs, _highestAllowedMinimumPingMs)
                .WithMessage($@"""The MinimumPingMs threshold must be >= {_lowestAllowedMinimumPingMs} and <= {_highestAllowedMinimumPingMs}. 
                                  It also must be less than the AveragePingMs and MaximumPingMs thresholds."""); 

            RuleFor(ptc => ptc.AveragePingMs)
                .GreaterThan(ptc => ptc.MinimumPingMs)
                .LessThan(ptc => ptc.MaximumPingMs)
                .InclusiveBetween(_lowestAllowedAveragePingMs, _highestAllowedAveragePingMs)
                .WithMessage($@"""The AveragePingMs threshold must be >= {_lowestAllowedAveragePingMs} and <= {_highestAllowedAveragePingMs}. 
                                  It also must be greater than the MinimumPingMs threshold, and less than the MaximumPingMs thresholds."""); 

            RuleFor(ptc => ptc.MaximumPingMs)
                .GreaterThan(ptc => ptc.MinimumPingMs)
                .GreaterThan(ptc => (short) ptc.AveragePingMs)
                .InclusiveBetween(_lowestAllowedMaximumPingMs, _highestAllowedMaximumPingMs)
                .WithMessage($@"""The MinimumPingMs threshold must be >= {_lowestAllowedMaximumPingMs} and <= {_highestAllowedMaximumPingMs}. 
                                  It also must be more than the MinimumPingMs and AveragePingMs thresholds."""); 

            RuleFor(ptc => ptc.JitterMs)
                .LessThan(ptc => ptc.AveragePingMs)
                .InclusiveBetween(_lowestAllowedJitterThresholdMs, _highestAllowedJitterThresholdMs)
                .WithMessage($@"""The JitterMs threshold must be >= {_lowestAllowedJitterThresholdMs} and <= {_highestAllowedJitterThresholdMs}. 
                                  It also must be less than the AveragePingMs and MaximumPingMs thresholds."""); 

            RuleFor(ptc => (double) ptc.PacketLossPercentage)
                .InclusiveBetween(_lowestAllowedPacketLossPercentage, _highestAllowedPacketLossPercentage)
                .WithMessage($"The PacketLossPercentage threshold must be between {_lowestAllowedPacketLossPercentage} and {_highestAllowedPacketLossPercentage}");
        }

    }
}

