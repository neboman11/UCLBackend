using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCLBackend.DataAccess.Models
{
    public class Player
    {
        public string PlayerID { get; set; }
        public string DiscordID { get; set; }
        public string Name { get; set; }
        #nullable enable
        [DisplayFormat(NullDisplayText = "None")]
        public string? PeakRank { get; set; }
        [DisplayFormat(NullDisplayText = "None")]
        public string? CurrentRank { get; set; }
        [DisplayFormat(NullDisplayText = "None")]
        public string? League { get; set; }
        [DisplayFormat(NullDisplayText = "None")]
        public string? Team { get; set; }
        [DisplayFormat(NullDisplayText = "None")]
        public string? TimeZone { get; set; }
        #nullable disable
        public int? PeakMMR { get; set; }
        public double? Salary { get; set; }
        public List<Account> Accounts { get; set; }
    }

    public class Account
    {
        public string AccountID { get; set; }
        public string Platform { get; set; }
        public string PlayerID { get; set; }
        public bool IsPrimary { get; set; }
        public Player Player { get; set; }
    }
}