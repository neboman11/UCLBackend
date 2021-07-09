using System;

namespace UCLBackend.Service.Data.Requests
{
    public class AddPlayerRequest
    {
        public ulong DiscordID { get; set; }
        public string PlayerName { get; set; }
        public string Platform { get; set; }
        public string Region { get; set; }
        public string RLTrackerLink { get; set; }
        public string[] AltRLTrackerLinks { get; set; }
    }
}