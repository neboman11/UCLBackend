using System.Collections.Generic;

namespace UCLBackend.Service.Data.Discord
{
    public class Member
    {
        public User User { get; set; }
        public List<ulong> Roles { get; set; }
    }
}
