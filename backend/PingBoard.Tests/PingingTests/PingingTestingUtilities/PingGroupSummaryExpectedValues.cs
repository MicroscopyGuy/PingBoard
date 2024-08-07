﻿using PingBoard.Database.Models;

namespace PingBoard.Tests.PingingTests.PingingTestingUtilities;
using PingBoard.Pinging;
using System.Reflection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;


public static class PingGroupSummaryExpectedValues
{
    public static readonly Dictionary<string, PingGroupSummary> ExpectedSummaries;

    static PingGroupSummaryExpectedValues()
    {
        string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        string filePath = Path.Combine(
            assemblyPath, 
            @"PingingTests\PingingTestingUtilities\PingGroupSummaryExpectedValues.json"
            );
            
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"The file can't be found. Current directory is: {Environment.CurrentDirectory}");
        }

        string jsonText = File.ReadAllText(filePath);
        ExpectedSummaries = JsonConvert.DeserializeObject<Dictionary<string, PingGroupSummary>>(jsonText);
         
    }

    public static void AssertExpectedValues(PingGroupSummary expected, PingGroupSummary actual)
    {
        Assert.Equal(expected.Target, actual.Target);
        Assert.Equal(expected.MinimumPing, actual.MinimumPing);
        Assert.Equal(expected.AveragePing, actual.AveragePing);
        Assert.Equal(expected.MaximumPing, actual.MaximumPing);
        Assert.Equal(expected.Jitter, actual.Jitter);
        Assert.Equal(expected.PacketLoss, actual.PacketLoss);
        Assert.Equal(expected.PacketsLost, actual.PacketsLost);
        Assert.Equal(expected.PacketsSent, actual.PacketsSent);
        Assert.Equal(expected.ConsecutiveTimeouts, actual.ConsecutiveTimeouts);
        Assert.Equal(expected.ExcludedPings, actual.ExcludedPings);
        Assert.Equal(expected.LastAbnormalStatus, actual.LastAbnormalStatus);
        Assert.Equal(expected.TerminatingIPStatus, actual.TerminatingIPStatus);
    }
    
}