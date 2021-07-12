using System;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using UCLBackend.Service.Services.Interfaces;

namespace UCLBackend.Service.Services
{
    // TODO: use async methods
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

        public void StoreValue(string key, string value)
        {
            _database.StringSet(key, value);
            _database.KeyExpire(key, TimeSpan.FromMinutes(60));
        }

        public string RetrieveValue(string key)
        {
            return _database.StringGet(key);
        }

        public void RemoveValue(string key)
        {
            _database.KeyDelete(key);
        }
    }
}
