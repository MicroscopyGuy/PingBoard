using Microsoft.Extensions.Options;

namespace PingBoard.Tests.MonitoringTests.ConfigurationTests;
using PingBoard.Monitoring.Configuration;
using PingBoard.Pinging.Configuration;
using PingingLimits = PingBoard.Pinging.Configuration.PingingBehaviorConfigLimits;
using MonitoringLimits = PingBoard.Monitoring.Configuration.MonitoringBehaviorConfigLimits;
using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Results;

public class MonitoringBehaviorConfigValidatorTest{
    [Fact]
    public void BehaviorIsValidWhenOutageAfterTimeoutsAtUpperLimit(){
        PingingLimits testLimits = new PingingBehaviorConfigLimits();

        // Since the MonitoringBehavior validation involves PingingBehaviorConfigInfo
        PingingBehaviorConfig testPingingBehavior = new PingingBehaviorConfig{
            PayloadStr   = "DoesntMatterForThisTest",
            PingsPerCall = PingingLimits.MostAllowedPingsPerCall, // somewhat matters, since ReportBackAfter is related
            TimeoutMs    = PingingLimits.LongestAllowedTimeoutMs,
            Ttl          = PingingLimits.LongestAllowedTtl,
            WaitMs       = PingingLimits.LongestAllowedWaitMs,
            ReportBackAfterConsecutiveTimeouts = testLimits.MostAllowedConsecutiveTimeoutsBeforeReportBack // matters
        };

        MonitoringBehaviorConfig testMonitoringBehavior = new MonitoringBehaviorConfig{
            OutageAfterTimeouts = MonitoringLimits.MostAllowedConsecutiveTimeoutsBeforeOutage
        };
        var testPingingConfig = Options.Create(testPingingBehavior);
        MonitoringBehaviorConfigValidator behaviorValidator = new MonitoringBehaviorConfigValidator(testPingingConfig);
        TestValidationResult<MonitoringBehaviorConfig> testResult = behaviorValidator.TestValidate(testMonitoringBehavior);
        testResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void BehaviorIsValidWhenOutageAfterTimeoutsAtLowerLimit(){
        PingingLimits testLimits = new PingingBehaviorConfigLimits();

        // Since the MonitoringBehavior validation involves PingingBehaviorConfigInfo
        PingingBehaviorConfig testPingingBehavior = new PingingBehaviorConfig{
            PayloadStr   = "DoesntMatterForThisTest",
            PingsPerCall = PingingLimits.FewestAllowedPingsPerCall, // somewhat matters, since ReportBackAfter is related
            TimeoutMs    = PingingLimits.ShortestAllowedTimeoutMs,
            Ttl          = PingingLimits.ShortestAllowedTtl,
            WaitMs       = PingingLimits.ShortestAllowedWaitMs,
            ReportBackAfterConsecutiveTimeouts = testLimits.MostAllowedConsecutiveTimeoutsBeforeReportBack // matters
        };

        MonitoringBehaviorConfig testMonitoringBehavior = new MonitoringBehaviorConfig{
            OutageAfterTimeouts = MonitoringLimits.MostAllowedConsecutiveTimeoutsBeforeOutage
        };

        var testPingingConfig = Options.Create(testPingingBehavior);
        MonitoringBehaviorConfigValidator behaviorValidator = new MonitoringBehaviorConfigValidator(testPingingConfig);
        TestValidationResult<MonitoringBehaviorConfig> testResult = behaviorValidator.TestValidate(testMonitoringBehavior);
        testResult.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void BehaviorIsInValidWhenOutageAfterTimeoutsIsAboveUpperLimit(){
        PingingLimits testLimits = new PingingBehaviorConfigLimits();

        // Since the MonitoringBehavior validation involves PingingBehaviorConfigInfo
        PingingBehaviorConfig testPingingBehavior = new PingingBehaviorConfig{
            PayloadStr   = "DoesntMatterForThisTest",
            PingsPerCall = PingingLimits.MostAllowedPingsPerCall+1, // somewhat matters, since ReportBackAfter is related
            TimeoutMs    = PingingLimits.LongestAllowedTimeoutMs+1,
            Ttl          = PingingLimits.LongestAllowedTtl+1,
            WaitMs       = PingingLimits.LongestAllowedWaitMs+1,
            ReportBackAfterConsecutiveTimeouts = testLimits.MostAllowedConsecutiveTimeoutsBeforeReportBack+1 // matters
        };

        MonitoringBehaviorConfig testMonitoringBehavior = new MonitoringBehaviorConfig{
            OutageAfterTimeouts = MonitoringLimits.MostAllowedConsecutiveTimeoutsBeforeOutage
        };
        
        var testPingingConfig = Options.Create(testPingingBehavior);
        MonitoringBehaviorConfigValidator behaviorValidator = new MonitoringBehaviorConfigValidator(testPingingConfig);
        TestValidationResult<MonitoringBehaviorConfig> testResult = behaviorValidator.TestValidate(testMonitoringBehavior);
        testResult.ShouldHaveValidationErrorFor(test => test.OutageAfterTimeouts);
    }
    
    [Fact]
    public void BehaviorIsInValidWhenOutageAfterTimeoutsIsBelowLowerLimit(){
        PingingLimits testLimits = new PingingBehaviorConfigLimits();

        // Since the MonitoringBehavior validation involves PingingBehaviorConfigInfo
        PingingBehaviorConfig testPingingBehavior = new PingingBehaviorConfig{
            PayloadStr   = "DoesntMatterForThisTest",
            PingsPerCall = PingingLimits.FewestAllowedPingsPerCall-1, // somewhat matters, since ReportBackAfter is related
            TimeoutMs    = PingingLimits.ShortestAllowedTimeoutMs-1,
            Ttl          = PingingLimits.ShortestAllowedTtl-1,
            WaitMs       = PingingLimits.ShortestAllowedWaitMs-1,
            ReportBackAfterConsecutiveTimeouts = testLimits.MostAllowedConsecutiveTimeoutsBeforeReportBack+1 // matters
        };
        var testPingingConfig = Options.Create(testPingingBehavior);
        MonitoringBehaviorConfig testMonitoringBehavior = new MonitoringBehaviorConfig{
            OutageAfterTimeouts = MonitoringLimits.MostAllowedConsecutiveTimeoutsBeforeOutage
        };
        
        MonitoringBehaviorConfigValidator behaviorValidator = new MonitoringBehaviorConfigValidator(testPingingConfig);
        TestValidationResult<MonitoringBehaviorConfig> testResult = behaviorValidator.TestValidate(testMonitoringBehavior);
        testResult.ShouldHaveValidationErrorFor(test => test.OutageAfterTimeouts);
    }
}