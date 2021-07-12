using System.Collections.Generic;

namespace UCLBackend.DataAccess.Models.Responses
{
    public class GetBallchasingGroupResponse
    {
        public List<GetBallchasingGroupResponsePlayer> Players { get; set; }
    }

    public class GetBallchasingGroupResponsePlayer
    {
        public string Platform { get; set; }
        public string Id { get; set; }
        public GetBallchasingGroupResponseCumulative Cumulative { get; set; }
    }

    public class GetBallchasingGroupResponseCumulative
    {
        public GetBallchasingGroupResponseCore Core { get; set; }
    }

    public class GetBallchasingGroupResponseCore
    {
        public int Shots { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Saves { get; set; }
        public int Score { get; set; }
    }
}
