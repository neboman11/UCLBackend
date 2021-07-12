using System.Threading.Tasks;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IReplayService
    {
        void BeginUploadProcess(ulong userId);
        void QueueReplay(ulong userId, string replayFileUrl);
        Task EndUploadProcess(ulong userId);
    }
}