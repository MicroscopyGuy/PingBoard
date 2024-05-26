using PingBoard.Pinging.Configuration;
using PingBoard.Pinging;
using Limits = PingBoard.Pinging.Configuration.PingingBehaviorConfigLimits;
using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Results;


namespace PingBoard.Tests.PingingTests.ConfigurationTests{
    public class PingingBehaviorConfigValidatorTest{
        /// <summary>
        /// Can slice this string to generate a string of a specified length, useful for faking the PayloadStr
        /// </summary>
        private readonly string PayloadStrSource = new String('@', Limits.LongestAllowedPayloadStr+1);

        [Fact]
        public void BehaviorIsValidWhenAllPropertiesAtUpperLimits(){
            Limits testLimits = new PingingBehaviorConfigLimits();
            PingingBehaviorConfig testConfig = new PingingBehaviorConfig{
                PayloadStr   = PayloadStrSource.Substring(0, Limits.LongestAllowedPayloadStr),
                PingsPerCall = Limits.MostAllowedPingsPerCall,
                TimeoutMs    = Limits.LongestAllowedTimeoutMs,
                Ttl          = Limits.LongestAllowedTtl,
                WaitMs       = Limits.LongestAllowedWaitMs,
                ReportBackAfterConsecutiveTimeouts = testLimits.MostAllowedConsecutiveTimeoutsBeforeReportBack
            };

            PingingBehaviorConfigLimits behaviorLimits = new PingingBehaviorConfigLimits(); 
            PingingBehaviorConfigValidator behaviorValidator = new PingingBehaviorConfigValidator(behaviorLimits);
            TestValidationResult<PingingBehaviorConfig> result = behaviorValidator.TestValidate(testConfig);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void BehaviorIsValidWhenAllPropertiesAtlowerLimits(){
            Limits testLimits = new PingingBehaviorConfigLimits();
            PingingBehaviorConfig testConfig = new PingingBehaviorConfig{
                PayloadStr   = PayloadStrSource.Substring(0,Limits.ShortestAllowedPayloadStr),
                PingsPerCall = Limits.FewestAllowedPingsPerCall,
                TimeoutMs    = Limits.ShortestAllowedTimeoutMs,
                Ttl          = Limits.ShortestAllowedTtl,
                WaitMs       = Limits.ShortestAllowedWaitMs,
                ReportBackAfterConsecutiveTimeouts = testLimits.FewestAllowedConsecutiveTimeoutsBeforeReportBack
            };

            PingingBehaviorConfigLimits behaviorLimits = new PingingBehaviorConfigLimits(); 
            PingingBehaviorConfigValidator behaviorValidator = new PingingBehaviorConfigValidator(behaviorLimits);
            TestValidationResult<PingingBehaviorConfig> result = behaviorValidator.TestValidate(testConfig);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void BehaviorIsInvalidWhenAllPropertiesBelowLimits(){
            Limits testLimits = new PingingBehaviorConfigLimits();
            PingingBehaviorConfig testConfig = new PingingBehaviorConfig{
                PayloadStr   = "",
                PingsPerCall = Limits.FewestAllowedPingsPerCall-1,
                TimeoutMs    = Limits.ShortestAllowedTimeoutMs-1,
                Ttl          = Limits.ShortestAllowedTtl-1,
                WaitMs       = Limits.ShortestAllowedWaitMs-1,
                ReportBackAfterConsecutiveTimeouts = testLimits.FewestAllowedConsecutiveTimeoutsBeforeReportBack-1
            };

            PingingBehaviorConfigLimits behaviorLimits = new PingingBehaviorConfigLimits(); 
            PingingBehaviorConfigValidator behaviorValidator = new PingingBehaviorConfigValidator(behaviorLimits);
            TestValidationResult<PingingBehaviorConfig> result = behaviorValidator.TestValidate(testConfig);
            result.ShouldHaveValidationErrorFor(test => test.PayloadStr);
            result.ShouldHaveValidationErrorFor(test => test.PingsPerCall);
            result.ShouldHaveValidationErrorFor(test => test.TimeoutMs);
            result.ShouldHaveValidationErrorFor(test => test.Ttl);
            result.ShouldHaveValidationErrorFor(test => test.WaitMs);
            result.ShouldHaveValidationErrorFor(test => test.ReportBackAfterConsecutiveTimeouts);    
        }

        [Fact]
        public void BehaviorIsInvalidWhenAllPropertiesAboveLimits(){
            Limits testLimits = new PingingBehaviorConfigLimits();
            PingingBehaviorConfig testConfig = new PingingBehaviorConfig{
                PayloadStr   = PayloadStrSource,
                PingsPerCall = Limits.MostAllowedPingsPerCall+1,
                TimeoutMs    = Limits.LongestAllowedTimeoutMs+1,
                Ttl          = Limits.LongestAllowedTtl+1,
                WaitMs       = Limits.LongestAllowedWaitMs+1,
                ReportBackAfterConsecutiveTimeouts = testLimits.MostAllowedConsecutiveTimeoutsBeforeReportBack+1 
            };

            PingingBehaviorConfigLimits behaviorLimits = new PingingBehaviorConfigLimits(); 
            PingingBehaviorConfigValidator behaviorValidator = new PingingBehaviorConfigValidator(behaviorLimits);
            TestValidationResult<PingingBehaviorConfig> result = behaviorValidator.TestValidate(testConfig);
            result.ShouldHaveValidationErrorFor(test => test.PayloadStr);
            result.ShouldHaveValidationErrorFor(test => test.PingsPerCall);
            result.ShouldHaveValidationErrorFor(test => test.TimeoutMs);
            result.ShouldHaveValidationErrorFor(test => test.Ttl);
            result.ShouldHaveValidationErrorFor(test => test.WaitMs);
            result.ShouldHaveValidationErrorFor(test => test.ReportBackAfterConsecutiveTimeouts);        
        }
    }
}