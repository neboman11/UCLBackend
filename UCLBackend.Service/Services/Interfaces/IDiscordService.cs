using System.Threading.Tasks;
using UCLBackend.Service.Data.Enums;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IDiscordService
    {
        Task AddLeagueRolesToUser(ulong discordId, PlayerLeague league);
    }
}