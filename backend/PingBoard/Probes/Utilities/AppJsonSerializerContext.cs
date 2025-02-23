namespace PingBoard.Probes.Utilities;

using System.Text.Json.Serialization;
using Apis.ProbeData;
using Microsoft.AspNetCore.Mvc;
using NetworkProbes.Common;
using NetworkProbes.Ping;
using Services;

[JsonSerializable(typeof(HostnameTarget))]
[JsonSerializable(typeof(IpAddressTarget))]
[JsonSerializable(typeof(ProbeConfigAggregate))]
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(PaginationToken<PingResultPublic>))]
[JsonSerializable(typeof(ListAnomaliesResponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
