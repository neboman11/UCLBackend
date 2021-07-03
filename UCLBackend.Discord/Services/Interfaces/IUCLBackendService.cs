using System.Threading.Tasks;

namespace UCLBackend.Discord.Services.Interfaces
{
    public interface IUCLBackendService
    {
        Task<bool> AddPlayer(ulong discordID, string username, string platform, string region, string rlTrackerLink, string[] altRLTrackerLinks);
    }
}