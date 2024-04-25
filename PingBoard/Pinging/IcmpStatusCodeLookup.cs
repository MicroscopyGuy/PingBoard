using System.Collections.Immutable;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace PingBoard.Pinging{
    public static class IcmpStatusCodeLookup{
        public static readonly IImmutableDictionary<IPStatus, IcmpStatusCodeEntry> StatusCodes;

        /// <summary>
        /// Static constructor, populates Errors with the IPStatus information from ICMPStatusCodes.json
        /// </summary>
        static IcmpStatusCodeLookup(){
            List<IcmpStatusCodeEntry> icmpStatusList = JsonSerializer.Deserialize<List<IcmpStatusCodeEntry>>("ICMPStatusCodes.json")!;
            StatusCodes = icmpStatusList.ToImmutableDictionary((icmpStatusEntry) => icmpStatusEntry.IcmpStatusCode);
        }       
        
        
        /// <summary>
        /// Reads in the IPStatus information from the ICMPStatusCodes.json folder 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns> the file contents as a string </returns>
        private static string ReadInIpStatusInfo(string filename){
            var type = typeof(IcmpStatusCodeLookup); // to get namespace and assembly name

            // use ErrorCodeLookup (can choose any class in this namespace) to get the namespace
            var fullEmbeddedFileName = $"{type.Namespace}.{filename}"; 
            
            // fullEmbeddedFileName requires namespace for access, and now use the type to get the
            // assembly, and pass fullEmbeddedFileName to GetManifestResourceStream to access the file
            using var stream = type.Assembly.GetManifestResourceStream(fullEmbeddedFileName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }


    }
}