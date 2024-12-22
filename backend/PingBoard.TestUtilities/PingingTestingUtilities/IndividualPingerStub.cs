namespace PingBoard.TestUtilities.PingingTestingUtilities;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using PingBoard.Pinging;
using Probes.NetworkProbes;

/// <summary>
/// This class should not be used for production, it is only used for testing.
/// <see cref="GroupPinger"/>.
/// All values used
///
/// FOR ADDITIONAL CONTEXT:
/// In the Ping class source code, SendPingAsync() begins a chain of function calls, and eventually calls
/// CreatePingReply which invokes either CreatePingReplyFromIcmpEchoReply, or CreatePingReplyFromIcmp6EchoReply.
/// Both functions create the PingReply objects returned by SendPing and SendPingAsync, and differ only
/// by their use for IPv4 and IPv6 requests, respectively.
///
/// Broadly, there are two cases these functions address, a returned PingReply.Status of IPStatus.Success,
/// and a returned PingReply.Status that is any of the other 23 possible IPStatus enums. Below is relevant
/// source code from the CreatePingReplyFromIcmpEchoReply() function in the System.Net standard library:
///**********************************************************************************************************
///| (source code below from CreatePingReplyFromIcmpEchoReply() in System.Net.NetworkInformation)           |
///| For the IpStatus.Success case:                                                                         |
///|     rtt = reply.roundTripTime;                                                                         |
///|     options = new PingOptions(reply.options.ttl, (reply.options.flags & DontFragmentFlag) > 0);        |
///|     buffer = new byte[reply.dataSize];                                                                 |
///|     Marshal.Copy(reply.data, buffer, 0, reply.dataSize);                                               |
///|                                                                                                        |
///| For the "any other," or "error" case:                                                                  |
///|     rtt = 0;                                                                                           |
///|     options = null;                                                                                    |
///|     buffer = Array.Empty byte(); //(angle brackets removed for xml comment formatting)                 |
///|*********************************************************************************************************
/// As shown, the round trip time, ping options, and returned buffer are only set if the ping was successful.
/// Otherwise, the round trip time is set to 0, the options are null, and the buffer is empty.
///
/// </summary>
public class IndividualPingerStub : IIndividualPinger
{
    /// <summary>
    /// The List of stubbed PingReply objects, to be returned by SendPingAsync in
    /// order of ascending index.
    /// </summary>
    private List<PingReply> _pingReplyStubs = [];

    /// <summary>
    /// for compatibility for testing
    /// </summary>
    private int _ttl;

    /// <summary>
    /// for compatibility for testing
    /// </summary>
    private int _timeoutMs;

    /// <summary>
    /// The index of next PingReply from _pingReplyStubs that should be returned by
    /// a SendPingAsync function call.
    /// </summary>
    private int _pingReplyIndex = 0;

    /// <summary>
    /// Populates the private _pingReplyStubs property with stubbed PingReply objects,
    /// constructed with the passed Lists of desired PingReply property values.
    ///
    /// For any index i, rtts[i], ipstatuses[i] and buffers[i] would be the
    /// RoundtripTime, Status, and Buffer values set on a single new PingReply object.
    ///
    /// This stubbed SendPingAsync function can then return each of the PingReply stubs in order,
    /// to make testing SendPingGroupAsync possible. <see cref="GroupPinger"/>
    ///
    /// </summary>
    /// <param name="rtts">A list of RoundtripTime property values for the PingReply objects to have set</param>
    /// <param name="ipstatuses">A list of Status property values for the PingReply objects to have set</param>
    /// <param name="buffers">A list of Buffer property values for the PingReply objects to have set</param>
    /// <param name="ipaddresses">A list of Address property values for the PingReply objects to have set</param>
    /// <param name="ttls">A list of ttl's for the PingReply's PingOptions objects to have set </param>
    public void PrepareStubbedPingReplies(
        List<long> rtts,
        List<IPStatus> ipstatuses,
        List<byte[]> buffers,
        List<string> ipaddresses,
        List<int> ttls
    )
    {
        for (int i = 0; i < rtts.Count; i++)
        {
            _pingReplyStubs.Add(
                MakePingReplyStub(rtts[i], ipstatuses[i], buffers[i], ipaddresses[i], ttls[i])
            );
        }
    }

    [ExcludeFromCodeCoverage]
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    static extern PingReply CallPingReplyConstructor(
        IPAddress address,
        PingOptions? options,
        IPStatus ipStatus,
        long rtt,
        byte[] buffer
    );

    public static PingReply MakePingReplyStub(
        long rtt,
        IPStatus status,
        byte[] buffer,
        string ipaddress,
        int ttl = 0,
        bool dontFragment = true
    )
    {
        var ip = ipaddress == "" ? IPAddress.Any : IPAddress.Parse(ipaddress);
        return (status != IPStatus.Success)
            ? CallPingReplyConstructor(ip, null, status, 0, [])
            : CallPingReplyConstructor(ip, MakePingOptions(ttl, dontFragment), status, rtt, buffer);
    }

    public static PingOptions MakePingOptions(
        int ttl = 0,
        bool dontFragment = true,
        bool makeNull = false
    )
    {
        return (makeNull) ? null : new PingOptions(ttl, dontFragment);
    }

    /// <summary>
    /// Returned the next stubbed Task|PingReply| object from the private _pingReplyStubs property,
    /// and then updates the index.
    ///
    /// Note: The values of the properties on the PingProbeInvocationParams object have absolutely no relevance here.
    /// The IndividualPingerStub class fakes output results, not input parameters. It is passed here for interface
    /// compliance only.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<PingReply> SendPingIndividualAsync(
        PingProbeInvocationParams pingParams,
        CancellationToken stoppingToken = default(CancellationToken)
    )
    {
        if (_pingReplyStubs.Count == 0)
        {
            throw new Exception(
                "_pingReplyStubs accessed without proper initialization by PrepareStubbedPingReplies()"
            );
        }

        await Task.Delay(1); // to return as Task<PingReply> instead of simply PingReply
        return _pingReplyStubs[_pingReplyIndex++ % _pingReplyStubs.Count]; // so we don't exhaust the data
    }

    public void SetTtl(int newTtl)
    {
        this._ttl = newTtl;
    }

    public void SetTimeout(int newTimeoutMs)
    {
        this._timeoutMs = newTimeoutMs;
    }

    public int GetTtl()
    {
        return this._ttl;
    }

    public int GetTimeout()
    {
        return this._timeoutMs;
    }
}
