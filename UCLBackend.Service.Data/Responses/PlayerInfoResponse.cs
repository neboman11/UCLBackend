namespace UCLBackend.Service.Data.Responses
{
    public class PlayerInfoResponse : BaseResponse
    {
        public double Salary { get; set; }
        public int PeakMMR { get; set; }
        public int CurrentMMR { get; set; }
    }
}
