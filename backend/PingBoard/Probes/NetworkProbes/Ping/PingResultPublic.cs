namespace PingBoard.Probes.NetworkProbes.Ping;

using System.Net.NetworkInformation;
using IpStatusCode;

/*
 * message PingResultPublic{
  google.protobuf.Timestamp start = 1;
  google.protobuf.Timestamp end = 2;
  int32 rtt = 3;
  string target = 4;
  string ipStatus = 5;
  int32 ttl = 6;
  string replyAddress = 7;
  string ipStatusShortMeaning = 8;
  string ipStatusOfficialMeaning = 9;
  string id = 10;
}
 */


public record PingResultPublic
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public long Rtt { get; set; }
    public string Target { get; set; }
    public int Ttl { get; set; }
    public string ReplyAddress { get; set; }
    public IcmpStatusCodeEntryPublic StatusCodeEntry { get; set; }
    public string Id { get; set; }
}
