using System.Net.NetworkInformation;
using PingBoard.Pinging;


namespace PingBoard.Pinging{
    public struct IcmpStatusCodeEntry{

        /// <summary>
        /// IPStatus is an enum defined in System.Net.NetworkInformation, and which represents
        /// the exact status of an individual ping sent via a ping function call 
        /// </summary>
        public IPStatus IcmpStatusCode { get; set;}

        // 
        /// <summary>
        /// A brief description of the ICMP StatusCode, intended for storage in DB and display in UI
        /// </summary>
        public string BriefDescription { get; set;} // brief, intended for DB/UI
        
        /// <summary>
        /// A more extensive description of the code, acquired from:
        /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ipstatus?view=net-8.0">
        ///     System.Net.NetworkInformation.IPStatus documentation
        /// </seealso> 
        /// </summary>

        public string ExtendedDescription { get; set;} 

        /// <summary>
        /// The resulting state of the monitoring warranted by the particular IPStatus returned by an *individual* ping
        /// </summary>
        public  PingingStates.PingState State {get; set;}
    }
}