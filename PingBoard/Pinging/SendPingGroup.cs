using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PingBoard.Pinging;

public class Pinger{

    public static async Task<PingGroupSummary> SendPingGroupAsync(int numberOfPings, int timeOut, int secsWaitForRetry, IPAddress target, int pingsPerCall){
        PingGroupSummary pingGroupInfo = PingGroupSummary.Empty();
        float[] responseTimes = new float[numberOfPings];

        using Ping pingSender = new Ping();
        int pingCounter = 0;
        while(pingCounter++ < numberOfPings){
            PingOptions options = new PingOptions();
            options.DontFragment = true; // prevents data from being split into > 1 packet, crucial
            string dataToSend = "@MIT_License@ https://github.com/MicroscopyGuy/PingBoard"; // make this configurable
            byte[] buffer = Encoding.ASCII.GetBytes(dataToSend);
            
            if (pingGroupInfo.TimeSendAttempt == DateTime.MinValue){
                pingGroupInfo.TimeSendAttempt = DateTime.Now;  // set time sent (attempted)
            }

            PingReply response = await pingSender.SendPingAsync(target, timeOut, buffer, options);
            PingingStates.PingState currentPingState = IcmpStatusCodeLookup.StatusCodes[response.Status].State;
            if (currentPingState == PingingStates.PingState.Continue){
                if (pingCounter == numberOfPings){ pingGroupInfo.TimeReceived = DateTime.Now; } // set time received

                pingGroupInfo.AveragePing += response.RoundtripTime;
                if (response.RoundtripTime < pingGroupInfo.MinimumPing){
                    pingGroupInfo.MinimumPing = response.RoundtripTime;
                }

                if (response.RoundtripTime > pingGroupInfo.MaximumPing){
                    pingGroupInfo.MaximumPing = response.RoundtripTime;
                }
                responseTimes[pingCounter] = response.RoundtripTime;
            }

            else if (response.Status == {
                
            }
        }

       
        // here need to store the information on the PingGroupSummary object -- should initialize earlier
        // need to add exception handling, response.Status interpretation per documentation: https://learn.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ping?view=net-8.0
        pingGroupInfo.Jitter = CalculatePingVariance(responseTimes, pingGroupInfo.AveragePing!.Value);
        pingGroupInfo.AveragePing /= pingCounter; 
        return pingGroupInfo;
    }

    public static float CalculatePingVariance(float[] responseTimes, float mean){
        if (responseTimes.Length <= 1){ 
            return 0;
        }

        double variance = 0;
        float deltaMean;
        foreach (float rtt in responseTimes){
            deltaMean = rtt-mean;
            variance += Math.Pow(deltaMean, 2);
        }

        variance = variance / responseTimes.Length;
        
        /* the narrowing conversion here is only a problem if:
           A) significant floating point precision is needed -- which isn't here
           B) there are impossibly long ping times -- which this application can't possibly have
           and so while it is technically dangerous, this function is fine for this application
        */
        return (float) variance;
    }

    /*


    

    

    

   

    
   
    

    

    

    

    

    

    

    

   

    

    

    

    

    


    



    
    ****************************************************************************CONTINUE************************************************************************
    ***Decision: Continue, Reason: This is what we want!***
    Success	0	
    The ICMP echo request succeeded; an ICMP echo reply was received. When you get this status code, the other PingReply
    properties contain valid data.

    ***Decision: Continue, Reason: Source computer has no gateway or router configured.***
    BadRoute	11012	
    The ICMP echo request failed because there is no valid route between the source and destination computers.

    ***Decision: continue, Reason: A router outside your network could not deliver the packet.***
    DestinationHostUnreachable	11003
    The ICMP echo request failed because the destination computer is not reachable.

    ***Decision: Continue, Reason: The network containing the target computer is unreachable."
    DestinationNetworkUnreachable	11002	
    The ICMP echo request failed because the network that contains the destination computer is not reachable.

    ***Decision: Continue, Reason: Port on target is in use***
    DestinationPortUnreachable	11005	
    The ICMP echo request failed because the port on the destination computer is not available.

    ***Decision: Continue, Reason: Unreachable target computer for unknown reason***
    DestinationUnreachable	11040	
    The ICMP echo request failed because the destination computer that is specified in an ICMP echo message is not reachable; the
    exact cause of problem is unknown.

    ***Decision: Continue, Reason: Ping timed out***
    TimedOut	11010	
    The ICMP echo Reply was not received within the allotted time. The default time allowed for replies is 5 seconds.
    You can change this value using the Send or SendAsync methods that take a timeout parameter.
    TOTAL: 7

    ******************************************************************************HALTS**************************************************************************
    ***Decision: Halt***
    Unknown -1 
    The ICMP echo request failed for an unknown reason.

    ***Decision: Halt, Reason: The ICMP echo request failed because the destination IP address cannot receive ICMP echo requests or should never appear 
    in the destination address field of any IP datagram.
    BadDestination	11018	
    The ICMP echo request failed because the destination IP address cannot receive ICMP echo requests or should never appear 
    in the destination address field of any IP datagram. For example, calling Send and specifying IP address "000.0.0.0" returns 
    this status.

    ***Decision: Halt, Reason: ICMP echo request failed because the header is invalid***
    BadHeader	11042	
    The ICMP echo request failed because the header is invalid.

    ***Decision: Halt, Reason: ICMP echo request failed because it contains an invalid option***
    BadOption	11007	
    The ICMP echo request failed because it contains an invalid option.

    ***Decision: Halt, Reason: ICMPv6 echo request failed because contact with the destination computer is administratively prohibited.***
    DestinationProhibited	11004	
    The ICMPv6 echo request failed because contact with the destination computer is administratively prohibited.
    This value applies only to IPv6.

    ***Decision: Halt, Reason: ICMP echo request The ICMP echo request failed because the destination computer that is specified in an ICMP echo message is not reachable***
    DestinationProtocolUnreachable	11004	
    The ICMP echo request failed because the destination computer that is specified in an ICMP echo message is not reachable, 
    because it does not support the packet's protocol. This value applies only to IPv4. This value is described in IETF RFC 1812 
    as Communication Administratively Prohibited.

    ***Decision: Halt, Reason: The ICMP echo request failed because the packet containing the request is larger than the MTU of a node between the source and destination.***
    PacketTooBig	11009	
    The ICMP echo request failed because the packet containing the request is larger than the maximum transmission unit (MTU)
    of a node (router or gateway) located between the source and destination. The MTU defines the maximum size of a transmittable packet.

    ***Decision: Halt, Reason: ICMP echo request's packet was discarded, either source computer's output queue full or packets sent too quickly for host to process them."***
    SourceQuench	11016	
    The ICMP echo request failed because the packet was discarded. This occurs when the source computer's output queue has
    insufficient storage space, or when packets arrive at the destination too quickly to be processed.

    ***Decision: Halt, Reason: The ICMP echo request failed because its Time to Live (TTL) value reached zero, causing the forwarding node (router or gateway)
    to discard the packet. ***
    TimeExceeded	11041	
    The ICMP echo request failed because its Time to Live (TTL) value reached zero, causing the forwarding node (router or gateway)
    to discard the packet.
    
    ***Decision: Halt, Reason: The ICMP echo request failed because its Time to Live (TTL) value reached zero, causing the forwarding node (router or gateway)
    to discard the packet. ***
    TtlExpired	11013	
    The ICMP echo request failed because its Time to Live (TTL) value reached zero, causing the forwarding node (router or gateway)
    to discard the packet.
    
    ***Decision: Halt, Reason: Packet was divided into fragments for transmission and not reassembled within the allotted time.  (60 seconds)***
    TtlReassemblyTimeExceeded	11014	
    The ICMP echo request failed because the packet was divided into fragments for transmission and all of the fragments were not
    received within the time allotted for reassembly. RFC 2460 specifies 60 seconds as the time limit within which all packet fragments must be received.

    ***Decision: Halt, Reason: Unknown network protocol in network header***
    UnrecognizedNextHeader	11043	(suspected halt)
    The ICMP echo request failed because the Next Header field does not contain a recognized value. The Next Header field indicates the extension
    header type (if present) or the protocol above the IP layer, for example, TCP or UDP.

    ParameterProblem	11015	
    The ICMP echo request failed because a node (router or gateway) encountered problems while processing the packet header. 
    This is the status if, for example, the header contains invalid field data or an unrecognized option.    

    ***Decision: Halt, Reason: Scope mismatch, possible configuration issue."***
    DestinationScopeMismatch	11045	
    The ICMP echo request failed because the source address and destination address that are specified in an ICMP echo message are
    not in the same scope. This is typically caused by a router forwarding a packet using an interface that is outside the scope of
    the source address. Address scopes (link-local, site-local, and global scope) determine where on the network an address is valid.

    ***Decision: Halt, Reason: Local NIC issue.***
    HardwareError	11008 (suspected halt)
    The ICMP echo request failed because of a hardware error.

    
    ***Decision: Halt, Reason: ICMP Error, unassigned error code.***
    IcmpError	11044	(suspected halt)
    The ICMP echo request failed because of an ICMP protocol error.

    
    ***Decision: Halt, Reason: The ICMP echo request failed because of insufficient network resources.***
    NoResources	11006	(suspected halt)
    The ICMP echo request failed because of insufficient network resources.
   
    TOTAL: 17
    */
}