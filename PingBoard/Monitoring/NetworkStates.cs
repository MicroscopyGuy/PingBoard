namespace PingBoard.Monitoring{
    public static class NetworkStates{
        /// <summary>
        ///  Provides a set of states which characterize the network at a point in time
        /// </summary>
        public enum NetworkState{
            Outage, // if in the middle of a network outage

            OutageBegin // if first detection of outage
        }
    }
}