namespace PingBoard.Probes.Services;

using System.Text.Json.Serialization;
using Apis.ProbeData;
using Microsoft.AspNetCore.Mvc;
using NetworkProbes.Common;
using NetworkProbes.Ping;
using PingBoard.Services.ServerEvents;
using Services;
using Utilities;

[JsonSerializable(typeof(HostnameTarget))]
[JsonSerializable(typeof(IpAddressTarget))]
[JsonSerializable(typeof(ProbeConfigAggregate))]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(PaginationToken<PingResultPublic>))]
[JsonSerializable(typeof(ListAnomaliesResponse))]
[JsonSerializable(typeof(ServerEventBase))]
[JsonSerializable(typeof(ProbeResult))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
