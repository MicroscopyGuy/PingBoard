using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using PingBoard.Pinging;


namespace PingBoard.Pinging{
    /// <summary>
    /// A struct to store information related to a single IcmpStatusCode, to be used for later lookup and translation.
    /// </summary>
    public struct IcmpStatusCodeEntry{

        /// <summary>
        /// IPStatus is an enum defined in System.Net.NetworkInformation, and which represents
        /// the exact status of an individual ping sent via a ping function call 
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IPStatus IcmpStatusCode { get; set;}

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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public  PingingStates.PingState State {get; set;}
    }
}