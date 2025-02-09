namespace PingBoard.Endpoints;

using System.Collections.Immutable;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using JsonSchemaGeneratedTypes;
using PingBoard.Database.Utilities;
using PingBoard.Endpoints;
using PingBoard.Probes.NetworkProbes;
using Probes.NetworkProbes.Common;
using Probes.NetworkProbes.Ping;
using Probes.Services;
using Probes.Utilities;
using Protos;

public static class ProbeInteropLayer
{
    // csharpier-ignore
    private static ProbeConfigAggregate PingProbeConfigFromProbeRequest(ProbeSchema probeConfigInfo)
    {
        if (!probeConfigInfo.TryGetAsPingProbeConfigSchema(out var jsonRequest))
        {
            throw new ArgumentException("Invalid probe operation");
        }

        var ipTarget = jsonRequest.Target.IsIpAddressTargetSchema;
        var configuredTarget = ipTarget
            ? jsonRequest.Target.AsIpAddressTargetSchema.AsString.GetString()!
            : jsonRequest.Target.AsHostnameTargetSchema.AsString.GetString()!;

        var pingBehavior = new PingProbeBehavior(
            ipTarget ? new IpAddressTarget(configuredTarget) : new HostnameTarget(configuredTarget),
            (int)jsonRequest.Ttl.AsNumber.AsDouble(),
            (int)jsonRequest.Timeout.AsNumber.AsDouble(),
            jsonRequest.PacketPayload.AsString.GetString()!
        );

        var pingThresholds = new PingProbeThresholds((long)jsonRequest.Timeout.AsNumber.AsDouble());

        var probeSchedule = new ProbeSchedule(
            TimeSpan.FromMilliseconds((long)jsonRequest.ProbeInterval.AsNumber.AsDouble())
        );

        return new ProbeConfigAggregate(pingBehavior, pingThresholds, probeSchedule);
    }

    public static ProbeConfigAggregate ProbeRequestJsonToConfigObjects(
        ProbeSchema probeConfigInfo,
        string probeOperation
    )
    {
        switch (probeOperation)
        {
            case "ping":
                return PingProbeConfigFromProbeRequest(probeConfigInfo);

            default:
                throw new ArgumentException("Invalid probe operation");
        }
    }
}
