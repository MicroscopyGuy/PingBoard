namespace PingBoard.Apis.ProbeData;

using PingBoard.Probes.NetworkProbes.Ping;

public class ListAnomaliesResponse
{
    public List<PingResultPublic> Anomalies { get; set; }
    public string PaginationToken { get; set; }
}
