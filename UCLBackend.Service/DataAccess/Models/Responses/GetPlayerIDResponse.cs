namespace UCLBackend.DataAccess.Models.Responses
{
    public class GetPlayerIDResponse
    {
        public GetPlayerIDResponseData Data { get; set; }
    }

    public class GetPlayerIDResponseData
    {
        public GetPlayerIDResponseMetaData MetaData { get; set; }
    }

    public class GetPlayerIDResponseMetaData
    {
        public string PlayerID { get; set; }
    }
}