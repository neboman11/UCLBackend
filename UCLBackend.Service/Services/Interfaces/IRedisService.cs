using System.Threading.Tasks;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IRedisService
    {
        void StoreValue(string key, string value);
        string RetrieveValue(string key);
        void RemoveValue(string key);
    }
}