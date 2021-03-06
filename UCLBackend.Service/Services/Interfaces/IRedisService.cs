using System;
using System.Threading.Tasks;

namespace UCLBackend.Service.Services.Interfaces
{
    public interface IRedisService
    {
        Task StoreValue(string key, string value);
        Task StoreValueWithExpiry(string key, string value, TimeSpan expiry);
        Task<string> RetrieveValue(string key);
        Task RemoveValue(string key);
        Task<bool> KeyExists(string key);
    }
}