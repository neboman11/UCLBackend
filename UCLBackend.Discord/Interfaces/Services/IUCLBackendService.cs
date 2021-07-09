using System.Threading.Tasks;

namespace UCLBackend.Discord.Interfaces.Services
{
    public interface IUCLBackendService
    {
        Task<bool> AddPlayer(ulong discordID, string playername, string region, string rlTrackerLink, string[] altRLTrackerLinks);
    }
}