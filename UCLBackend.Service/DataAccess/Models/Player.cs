using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCLBackend.DataAccess.Models
{
    [Table("Players")]
    public class Player
    {
        public string PlayerID { get; set; }
        public string DiscordID { get; set; }
        public string Name { get; set; }
        public int? TeamID { get; set; }
        #nullable enable
        public Team? Team { get; set; }
        #nullable disable
        public bool? IsFreeAgent { get; set; }
        public int? PeakMMR { get; set; }
        public int? CurrentMMR { get; set; }
        public double? Salary { get; set; }
        public List<Account> Accounts { get; set; }
    }

    [Table("Accounts")]
    public class Account
    {
        public string AccountID { get; set; }
        public string Platform { get; set; }
        public bool IsPrimary { get; set; }
        public string PlayerID { get; set; }
        public Player Player { get; set; }
    }
}