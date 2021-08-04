using UCLBackend.Service.Data.Enums;

namespace UCLBackend.Service.Data.Requests
{
    public class DraftRequest : BaseRequest
    {
        public PlayerFranchise Franchise { get; set; }
    }
}
