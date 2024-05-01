using System.Collections.Immutable;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace PingBoard.Pinging{
    public static class IcmpStatusCodeLookup{
        /// <summary>
        /// Immutable data container allowing each IPStatus enum to reference its associated info
        /// <seealso> **Important:** Note that the DestinationProhibited and DestinationProtocolUnreachable enums
        ///                          both have the same ordinal value. This causes a System.TypeInitializationException when
        ///                          deserializing this data into StatusCodes, the IImmutableDictionary. Since the Dictionary 
        ///                          checks these ordinal values for uniqueness (in this case, both 11004), they are considered duplicates.
        ///                          
        ///                          The DestinationProhibited enum describes a prohibited contact with a target computer, and is ONLY for use with IPv6.
        ///                          The DestinationProtocolUnreachable is similar, but uses IPv4. Since this application is only using IPv4 -- for now --
        ///                          I have removed the DestinationProhibited entry from ICMPStatusCodes.json file.
        ///                          
        ///                          If in the future this enum becomes relevant, ie, if IPv6 support is added, then I will perhaps create my own set of enums
        ///                          and map these IPStatus enums to them, to avoid this duplication.  
        ///   </seealso>
        /// </summary>
        public static readonly IImmutableDictionary<IPStatus, IcmpStatusCodeEntry> StatusCodes;

        /// <summary>
        /// Static constructor, populates StatusCodes with the IPStatus information from ICMPStatusCodes.json
        /// </summary>
        static IcmpStatusCodeLookup(){
            string statusCodeInfoFromJson = ReadInIpStatusInfo("ICMPStatusCodes.json");
            List<IcmpStatusCodeEntry> icmpStatusList = JsonSerializer.Deserialize<List<IcmpStatusCodeEntry>>(statusCodeInfoFromJson)!;
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
            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        }


    }
}