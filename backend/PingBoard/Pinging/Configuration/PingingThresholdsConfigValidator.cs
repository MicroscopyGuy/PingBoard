using Microsoft.Extensions.Options;
using FluentValidation;
using System.Reflection.Metadata;
using PingBoard.Pinging;
using Limits = PingBoard.Pinging.Configuration.PingingThresholdsConfigLimits;

namespace PingBoard.Pinging.Configuration{
    /// <summary>
    /// Validates the values put in the PingingThresholds section of the appsettings.json file 
    /// </summary>
    public class PingingThresholdsConfigValidator : AbstractValidator<PingingThresholdsConfig>{
        public PingingThresholdsConfigValidator(PingingThresholdsConfigLimits thresholdsLimits){
            RuleFor(ptc => ptc.MinimumPingMs)
                .LessThan(ptc => (short?) ptc.AveragePingMs)
                .InclusiveBetween(Limits.LowestAllowedMinimumPingMs, Limits.HighestAllowedMinimumPingMs)
                .WithMessage($"""
                                The MinimumPingMs threshold must be >= {Limits.LowestAllowedMinimumPingMs} and <= {Limits.HighestAllowedMinimumPingMs}. 
                                It also must be less than the AveragePingMs and MaximumPingMs thresholds.
                              """); 

            RuleFor(ptc => ptc.AveragePingMs)
                .GreaterThan(ptc => ptc.MinimumPingMs)
                .LessThan(ptc => ptc.MaximumPingMs)
                .InclusiveBetween(Limits.LowestAllowedAveragePingMs, Limits.HighestAllowedAveragePingMs)
                .WithMessage($"""
                                The AveragePingMs threshold must be >= {Limits.LowestAllowedAveragePingMs} and <= {Limits.HighestAllowedAveragePingMs}. 
                                It also must be greater than the MinimumPingMs threshold, and less than the MaximumPingMs thresholds.
                              """); 

            RuleFor(ptc => ptc.MaximumPingMs)
                .GreaterThan(ptc => ptc.MinimumPingMs)
                .GreaterThan(ptc => (int) ptc.AveragePingMs)
                .InclusiveBetween(Limits.LowestAllowedMaximumPingMs, Limits.HighestAllowedMaximumPingMs)
                .WithMessage($"""
                                The MaximumPingMs threshold must be >= {Limits.LowestAllowedMaximumPingMs} and 
                                <= {Limits.HighestAllowedMaximumPingMs}. 
                                It also must be more than the MinimumPingMs and AveragePingMs thresholds.
                              """); 

            RuleFor(ptc => ptc.JitterMs)
                .LessThan(ptc => ptc.AveragePingMs)
                .InclusiveBetween(thresholdsLimits.LowestAllowedJitterThresholdMs, thresholdsLimits.HighestAllowedJitterThresholdMs)
                .WithMessage($"""
                                The JitterMs threshold must be >= {thresholdsLimits.LowestAllowedJitterThresholdMs} and
                                <= {thresholdsLimits.LowestAllowedJitterThresholdMs}. 
                                It also must be less than the AveragePingMs and MaximumPingMs thresholds.
                              """); 

            RuleFor(ptc => (double) ptc.PacketLossPercentage)
                .InclusiveBetween(Limits.LowestAllowedPacketLossPercentage, Limits.HighestAllowedPacketLossPercentage)
                .WithMessage($"""
                               The PacketLossPercentage threshold must be between 
                               {Limits.LowestAllowedPacketLossPercentage} and {Limits.HighestAllowedPacketLossPercentage}
                              """);
        }
    }
}

