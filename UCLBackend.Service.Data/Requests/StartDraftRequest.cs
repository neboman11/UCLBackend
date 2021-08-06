using UCLBackend.Service.Data.Enums;

namespace UCLBackend.Service.Data.Requests
{
    public class StartDraftRequest : BaseRequest
    {
        public PlayerLeague League { get; set; }
    }
}
