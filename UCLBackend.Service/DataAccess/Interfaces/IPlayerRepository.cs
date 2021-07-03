using System.Threading.Tasks;
using UCLBackend.DataAccess.Models;

namespace UCLBackend.Service.DataAccess.Interfaces
{
    public interface IPlayerRepository
    {
        void AddPlayer(Player player);
        void AddAccount(Account account);
        Task<string> RemoteGetPlayerID(string platform, string username);
    }
}