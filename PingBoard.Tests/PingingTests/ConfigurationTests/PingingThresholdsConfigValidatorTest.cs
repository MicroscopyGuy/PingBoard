using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using Limits = PingBoard.Pinging.Configuration.PingingThresholdsConfigLimits;
using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Results;

namespace PingBoard.Tests.PingingTests.ConfigurationTests{
    public class PingingThresholdsConfigValidatorTest{

        [Fact]
        public void ThresholdsAreValidWhenAllPropertiesAtUpperLimits(){
            PingingThresholdsConfigLimits testLimits = new PingingThresholdsConfigLimits();
            PingingThresholdsConfig testThresholds = new PingingThresholdsConfig{
                MinimumPingMs = Limits.HighestAllowedMinimumPingMs,
                AveragePingMs = Limits.HighestAllowedAveragePingMs,
                MaximumPingMs = Limits.HighestAllowedMaximumPingMs,
                JitterMs      = testLimits.HighestAllowedJitterThresholdMs,
                PacketLossPercentage = Limits.HighestAllowedPacketLossPercentage
            };

            PingingThresholdsConfigValidator thresholdsValidator = new PingingThresholdsConfigValidator(testLimits);
            TestValidationResult<PingingThresholdsConfig> result = thresholdsValidator.TestValidate(testThresholds);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void ThresholdsAreValidWhenAllPropertiesAtLowerLimits(){
            PingingThresholdsConfigLimits testLimits = new PingingThresholdsConfigLimits();
            PingingThresholdsConfig testThresholds = new PingingThresholdsConfig{
                MinimumPingMs = Limits.LowestAllowedMinimumPingMs,
                AveragePingMs = Limits.LowestAllowedAveragePingMs,
                MaximumPingMs = Limits.LowestAllowedMaximumPingMs,
                JitterMs      = testLimits.LowestAllowedJitterThresholdMs,
                PacketLossPercentage = Limits.LowestAllowedPacketLossPercentage
            };

            PingingThresholdsConfigValidator thresholdsValidator = new PingingThresholdsConfigValidator(testLimits);
            TestValidationResult<PingingThresholdsConfig> result = thresholdsValidator.TestValidate(testThresholds);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void ThresholdsAreInValidWhenAllPropertiesBelowLimits(){
            PingingThresholdsConfigLimits testLimits = new PingingThresholdsConfigLimits();
            PingingThresholdsConfig testThresholds = new PingingThresholdsConfig{
                MinimumPingMs = Limits.LowestAllowedMinimumPingMs-1,
                AveragePingMs = Limits.LowestAllowedAveragePingMs-1,
                MaximumPingMs = Limits.LowestAllowedMaximumPingMs-1,
                JitterMs      = testLimits.LowestAllowedJitterThresholdMs-1,
                PacketLossPercentage = Limits.LowestAllowedPacketLossPercentage-1
            };

            PingingThresholdsConfigValidator thresholdsValidator = new PingingThresholdsConfigValidator(testLimits);
            TestValidationResult<PingingThresholdsConfig> result = thresholdsValidator.TestValidate(testThresholds);
            result.ShouldHaveValidationErrorFor(test => test.MinimumPingMs);
            result.ShouldHaveValidationErrorFor(test => test.AveragePingMs);
            result.ShouldHaveValidationErrorFor(test => test.MaximumPingMs);
            result.ShouldHaveValidationErrorFor(test => test.JitterMs);
            result.ShouldHaveValidationErrorFor(test => test.PacketLossPercentage);
        }

        [Fact]
        public void ThresholdsAreInValidWhenAllPropertiesAboveLimits(){
            PingingThresholdsConfigLimits testLimits = new PingingThresholdsConfigLimits();
            PingingThresholdsConfig testThresholds = new PingingThresholdsConfig{
                MinimumPingMs = Limits.HighestAllowedMinimumPingMs+1,
                AveragePingMs = Limits.HighestAllowedAveragePingMs+1,
                MaximumPingMs = Limits.HighestAllowedMaximumPingMs+1,
                JitterMs      = testLimits.HighestAllowedJitterThresholdMs+1,
                PacketLossPercentage = Limits.HighestAllowedPacketLossPercentage+1
            };

            PingingThresholdsConfigValidator thresholdsValidator = new PingingThresholdsConfigValidator(testLimits);
            TestValidationResult<PingingThresholdsConfig> result = thresholdsValidator.TestValidate(testThresholds);
            result.ShouldHaveValidationErrorFor(test => test.MinimumPingMs);
            result.ShouldHaveValidationErrorFor(test => test.AveragePingMs);
            result.ShouldHaveValidationErrorFor(test => test.MaximumPingMs);
            result.ShouldHaveValidationErrorFor(test => test.JitterMs);
            result.ShouldHaveValidationErrorFor(test => test.PacketLossPercentage);
        }
    }
}