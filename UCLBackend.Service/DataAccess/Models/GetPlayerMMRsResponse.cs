using System;
using System.Collections.Generic;

namespace UCLBackend.DataAccess.Models
{
    public class GetPlayerMMRsResponse
    {
        public Dictionary<int, GetPlayerMMRsResponseMmrObject[]> Data { get; set; }
    }

    public class GetPlayerMMRsResponseMmrObject
    {
        public int Rating { get; set; }
        public DateTime CollectDate { get; set; }
    }
}
