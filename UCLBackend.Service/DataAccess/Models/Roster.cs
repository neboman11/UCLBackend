using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCLBackend.DataAccess.Models
{
    [Table("Roster")]
    public class Team
    {
        public int TeamID { get; set; }
        public string League { get; set; }
        public string TeamName { get; set; }
        public string Conference { get; set; }
        public double CapSpace { get; set; }
        List<Player> Players { get; set; }
    }
}