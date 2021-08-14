using System.Collections.Generic;
using UCLBackend.Service.Data.Enums;

namespace UCLBackend.Service.Data.Helpers
{
    public static class PlayerHelpers
    {
        public static PlayerLeague GetPlayerLeague(double salary, Dictionary<PlayerLeague, double> leagueMinSalaries)
        {
            if (salary < leagueMinSalaries[PlayerLeague.Ultra])
            {
                return PlayerLeague.Origins;
            }
            else if (salary < leagueMinSalaries[PlayerLeague.Elite])
            {
                return PlayerLeague.Ultra;
            }
            else if (salary < leagueMinSalaries[PlayerLeague.Superior])
            {
                return PlayerLeague.Elite;
            }
            else
            {
                return PlayerLeague.Superior;
            }
        }
    }
}
