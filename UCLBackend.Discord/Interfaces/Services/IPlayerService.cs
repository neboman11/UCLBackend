using System.Threading.Tasks;

namespace UCLBackend.Discord.Interfaces.Services
{
    public interface IPlayerService
    {
        Task AddPlayer(ulong discordID, string playername, string region, string rlTrackerLink, string[] altRLTrackerLinks);
        Task SignPlayer(ulong discordID, string franchiseName);
        Task ReleasePlayer(ulong discordID);
    }
}