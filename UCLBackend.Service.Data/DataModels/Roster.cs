using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCLBackend.Service.Data.DataModels
{
    [Table("Roster")]
    public class Team
    {
        public int TeamID { get; set; }
        public string League { get; set; }
        public string TeamName { get; set; }
        public string Conference { get; set; }
        public List<Player> Players { get; set; }
    }
}
