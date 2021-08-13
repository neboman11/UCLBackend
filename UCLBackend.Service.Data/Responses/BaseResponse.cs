using Newtonsoft.Json;

namespace UCLBackend.Service.Data.Responses
{
    public class BaseResponse
    {
        public bool HasError { get; set; }
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
