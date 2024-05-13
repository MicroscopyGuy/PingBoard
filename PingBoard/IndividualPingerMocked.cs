namespace PingBoard.Pinging;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;


public class IndividualPingerMocked : IIndividualPinger{
    private readonly PingReply _mockedReply;
    public IndividualPingerMocked(){
        Type pingReplyType = typeof(PingReply);

        _mockedReply = (PingReply)pingReplyType.Assembly.CreateInstance(
                    pingReplyType.FullName!,
                    false,
                    BindingFlags.Default | BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic,
                    null, 
                    new object?[] {IPAddress.Any, null, IPStatus.Success, 10, new byte[10]}!, 
                    null, 
                    null
        )!;
    }
    public Task<PingReply> SendPingIndividualAsync(IPAddress target){
        return Task.FromResult(_mockedReply);        
    }
}
