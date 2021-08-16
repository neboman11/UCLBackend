using System.ComponentModel.DataAnnotations.Schema;
using UCLBackend.Service.Data.DataModels;

namespace UCLBackend.Service.DataAccess.Models
{
    [Table("Standings")]
    public class Standing
    {
        public int StandingId { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Saves { get; set; }
        public int Shots { get; set; }
        public int Score { get; set; }
        public int PlayerID { get; set; }
        public Player Player { get; set; }
    }
}
