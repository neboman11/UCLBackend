using System;

namespace UCLBackend.Service.Data.Requests
{
    public class ChangePlayerNameRequest : BaseRequest
    {
        public string NewName { get; set; }
    }
}
