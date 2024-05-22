namespace PingBoard.Tests.PingingTests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PingBoard.Pinging;
using PingBoard.Pinging.Configuration;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Net;

/// <summary>
/// This class should not be used for production, it is only used for testing SendPingGroupAsync.
/// <see cref="GroupPinger"/>.
/// All values used 
///
/// FOR ADDITIONAL CONTEXT:
/// In the Ping class source code, /// SendPingAsync() begins a chain of function calls eventually calls
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
/// Otherwise the round trip time is set to 0, the options are null, and the buffer is empty.
/// 
/// </summary>                      
public class IndividualPingerStub : IIndividualPinger
{
    /// <summary>
    /// The List of stubbed PingReply objects, to be returned by SendPingAsync in
    /// order of ascending index.
    /// </summary>
    private List<PingReply> _pingReplyStubs;
    
    /// <summary>
    /// A (real) PingingBehaviorConfig object which contains values needed for the PingOptions object.
    /// </summary>
    private PingingBehaviorConfig _pingBehavior;
    
    /// <summary>
    /// The index of next PingReply from _pingReplyStubs that should be returned by
    /// a SendPingAsync function call. 
    /// </summary>
    private int _pingReplyIndex;


    public IndividualPingerStub(PingOptions options, IOptions<PingingBehaviorConfig> pingBehavior, 
        ILogger<IIndividualPinger> logger){
        _pingBehavior = pingBehavior.Value;
    }

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
    /// <param name="rtts"></param>
    /// <param name="ipstatuses"></param>
    /// <param name="buffers"></param>
    /// <param name="ipaddresses"></param>
    /// <param name="ttls"></param>
    public void PrepareStubbedPingReplies(List<long> rtts, List<IPStatus> ipstatuses, List<byte[]> buffers,
        List<IPAddress> ipaddresses, List<int> ttls) {
        for (int i = 0; i< rtts.Count; i++){
            _pingReplyStubs.Add(MakePingReplyStub(rtts[i], ipstatuses[i], buffers[i], ipaddresses[i], ttls[i]));
        }
    }
    
    public static PingReply MakePingReplyStub(long rtt, IPStatus status, byte[] buffer, 
                                              IPAddress ipaddress, int ttl = 0, bool dontFragment = true){
        BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.CreateInstance | 
                                    BindingFlags.NonPublic | BindingFlags.Instance;

        PingOptions optionsStub = MakePingOptionsStub(ttl, dontFragment);
        
        PingReply stub = (PingReply) Activator.CreateInstance(
            typeof(PingReply),
            bindingFlags,
            null,
            new object?[] { IPAddress.Any, optionsStub, status, rtt, buffer },
            null)!;

        return stub;
    }

    /// <summary>
    /// Returns a PingReply stub initialized with all values set to what they would be if the
    /// Ping was unsuccessful (with a random, Non-Success IPStatus, of course).
    /// See first comment at top of class definition for more information.
    /// </summary>
    /// <param name="status">The desired IPStatus for the reply to contain, Unknown by default</param>
    /// <returns>A stubbed PingReply object, reflective of an unsuccessful ping</returns>
    public static PingReply MakeEmptyReplyStub(IPStatus status = IPStatus.Unknown) {
        BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.CreateInstance | 
                                    BindingFlags.NonPublic | BindingFlags.Instance;
        
        PingReply stub = (PingReply) Activator.CreateInstance(
            typeof(PingReply),
            bindingFlags,
            null,
            new object?[] { IPAddress.Any, null, status, 0, new byte[]{} },
            null)!;

        return stub;
    }
    
    public static PingOptions MakePingOptionsStub(int ttl, bool dontFragment){
        BindingFlags bindingFlags = BindingFlags.Default | BindingFlags.CreateInstance | 
                                    BindingFlags.NonPublic | BindingFlags.Instance; 
     
        PingOptions stub = (PingOptions) Activator.CreateInstance(
            typeof(PingOptions),
            bindingFlags,
            null,
            new object?[] {ttl, dontFragment},
            null)!;

        return stub;
    }

    /// <summary>
    /// Returned the next stubbed PingReply object from the private _pingReplyStubs property,
    /// and then updates the index.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<PingReply> SendPingIndividualAsync(IPAddress target) {
        if (_pingReplyStubs.Count == 0) {
            throw new Exception("""
                                          _pingReplyStubs accessed without proper intialization by
                                          PrepareStubbedPingReplies() function call.
                                        """);
        }
        
        if (_pingReplyIndex >= _pingReplyStubs.Count) {
            throw new Exception("""
                                          There are no additional PingReply objects to be returned.
                                       """);
        }
        
        await Task.Delay(1); // to return as Task<PingReply> instead of simply PingReply
        return _pingReplyStubs[_pingReplyIndex++];
    }
}



