namespace PingBoard.Endpoints;

using System.Collections.Immutable;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using JsonSchemaGeneratedTypes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PingBoard.Database.Utilities;
using PingBoard.Endpoints;
using PingBoard.Probes.NetworkProbes;
using Probes.NetworkProbes.Common;
using Probes.NetworkProbes.Ping;
using Probes.Services;
using Probes.Utilities;
using Protos;
using StartProbe = JsonSchemaGeneratedTypes.StartProbeRequest;
using StopProbe = JsonSchemaGeneratedTypes.StopProbeRequest;

public static class ProbeInteropLayer
{
    // csharpier-ignore
    private static ProbeConfigAggregate PingProbeConfigFromProbeRequest(in PingProbeConfig pingProbeRequest)
    {
        var targetObj = pingProbeRequest.Target.AsRequiredTargetAndTargetType;
        var isIpTarget = targetObj.TargetType.AsString.GetString() == "IpAddress";
        var configuredTarget = targetObj.Target.AsString.GetString();

        var pingBehavior = new PingProbeBehavior(
            isIpTarget ? new IpAddressTarget(configuredTarget) : new HostnameTarget(configuredTarget),
            (int) pingProbeRequest.Ttl.AsNumber.AsDouble(),
            (int) pingProbeRequest.Timeout.AsNumber.AsDouble(), //AsNumber.AsDouble(),
            pingProbeRequest.PacketPayload.AsString.GetString()!
        );

        var pingThresholds = new PingProbeThresholds((long)pingProbeRequest.Timeout.AsNumber.AsDouble());

        var probeSchedule = new ProbeSchedule(
            TimeSpan.FromMilliseconds((long)pingProbeRequest.ProbeInterval.AsNumber.AsDouble())
        );

        var probeType = pingProbeRequest.ProbeType.AsString.GetString()!;
        return new ProbeConfigAggregate(probeType, pingBehavior, pingThresholds, probeSchedule);
    }

    private static ProbeConfigAggregate TracerouteProbeConfigFromProbeRequest(
        in TracerouteProbeConfig tracerouteProbeRequest
    )
    {
        throw new NotImplementedException();
    }

    public static ProbeConfigAggregate ProbeRequestJsonToConfigObjects(StartProbe probeConfigInfo)
    {
        var thing = probeConfigInfo.AsPingProbeConfig;

        return probeConfigInfo.Match(
            PingProbeConfigFromProbeRequest,
            TracerouteProbeConfigFromProbeRequest,
            (in StartProbe p) => throw new NotImplementedException()
        );
    }
}
