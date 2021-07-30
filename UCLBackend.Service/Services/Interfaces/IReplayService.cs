using System.Threading.Tasks;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IReplayService
    {
        Task BeginUploadProcess(ulong userId);
        Task QueueReplay(ulong userId, string replayFileUrl);
        Task EndUploadProcess(ulong userId);
    }
}