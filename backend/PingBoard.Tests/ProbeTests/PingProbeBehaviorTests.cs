namespace PingBoard.Tests.ProbeTests;

using System.Text.Json;
using Probes.NetworkProbes;
using Probes.NetworkProbes.Ping;

public class PingProbeBehaviorTests
{
    [Fact]
    public void CanDeserializeJsonIntoPingProbeBehavior_OnJsonWithIpTarget()
    {
        var jsonParams =
            $@"
            {{
                ""Target"":{{
                    ""ipAddress"": ""8.8.8.8"",
                    ""targetType"":""ipAddress""
                }},
                ""Ttl"": 64,
                ""Timeout"": 1000,
                ""PacketPayload"": ""This is the string I want""
            }}";

        var result = JsonSerializer.Deserialize<PingProbeBehavior>(
            jsonParams,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );
        Assert.NotNull(result);
    }

    [Fact]
    public void CanDeserializeJsonIntoPingProbeBehavior_OnJsonWithHostnameTarget()
    {
        var jsonParams =
            $@"
            {{
                ""Target"":{{
                    ""hostname"": ""google.com"",
                    ""targetType"":""hostname""
                }},
                ""Ttl"": 64,
                ""Timeout"": 1000,
                ""PacketPayload"": ""This is the string I want""
            }}";

        var result = JsonSerializer.Deserialize<PingProbeBehavior>(
            jsonParams,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );
        Assert.NotNull(result);
    }
}
