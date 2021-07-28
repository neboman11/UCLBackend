using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UCLBackend.DataAccess.Models.Responses
{
    public class GetPlayerMMRsResponse
    {
        public Dictionary<int, GetPlayerMMRsResponseMmrObject[]> Data { get; set; }
    }

    public class GetPlayerMMRsResponseMmrObject
    {
        [JsonProperty("rating")]
        public int Rating { get; set; }
        [JsonProperty("collectDate")]
        public DateTime CollectDate { get; set; }
    }
}
