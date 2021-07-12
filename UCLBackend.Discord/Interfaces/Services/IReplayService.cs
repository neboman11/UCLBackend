using System.Threading.Tasks;

namespace UCLBackend.Discord.Interfaces.Services
{
    public interface IReplayService
    {
        Task BeginUploadProcess(ulong userId);
        Task QueueReplay(ulong userId, string replayFileUrl);
        Task EndUploadProcess(ulong userId);
    }
}