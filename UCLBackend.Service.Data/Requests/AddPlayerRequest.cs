using System;

namespace UCLBackend.Service.Data.Requests
{
    public class AddPlayerRequest : BaseRequest
    {
        public string PlayerName { get; set; }
        public string RLTrackerLink { get; set; }
        public string[] AltRLTrackerLinks { get; set; }
    }
}
