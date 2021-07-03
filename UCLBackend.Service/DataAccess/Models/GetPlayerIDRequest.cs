namespace UCLBackend.DataAccess.Models
{
    public class GetPlayerIDRequest
    {
        public GetPlayerIDRequestData Data { get; set; }
    }

    public class GetPlayerIDRequestData
    {
        public GetPlayerIDRequestMetaData MetaData { get; set; }
    }

    public class GetPlayerIDRequestMetaData
    {
        public string PlayerID { get; set; }
    }
}