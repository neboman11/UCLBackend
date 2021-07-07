namespace UCLBackend.DataAccess.Models
{
    public class Team
    {
        public int TeamID { get; set; }
        public string League { get; set; }
        public string TeamName { get; set; }
        public string Conference { get; set; }
        public double CapSpace { get; set; }
        #nullable enable
        public Player? PlayerA { get; set; }
        public Player? PlayerB { get; set; }
        public Player? PlayerC { get; set; }
        public Player? Reserve { get; set; }
        #nullable disable
    }
}