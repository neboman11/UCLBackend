namespace UCLBackend.Service.Data.Responses
{
    public class BaseResponse
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
