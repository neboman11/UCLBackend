using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using UCLBackend.Service.Services.Interfaces;

namespace UCLBackend.Service.Services
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly IDatabase _database;

        public RedisService(IConfiguration configuration)
        {
            var redisHost = configuration.GetSection("Redis").GetValue<string>("Host");
            _connection = ConnectionMultiplexer.Connect(redisHost);

            _database = _connection.GetDatabase();
        }

        public async Task StoreValue(string key, string value)
        {
            await _database.StringSetAsync(key, value);
        }

        public async Task StoreValueWithExpiry(string key, string value, TimeSpan expiry)
        {
            await _database.StringSetAsync(key, value);
            await _database.KeyExpireAsync(key, expiry);
        }

        public async Task<string> RetrieveValue(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task RemoveValue(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> KeyExists(string key)
        {
            return await _database.KeyExistsAsync(key);
        }
    }
}
