using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using XPike.Configuration;
using XPike.Logging;
using XPike.Redis;

namespace XPike.Caching.Redis
{
    public class RedisCachingProvider
        : IRedisCachingProvider
    {
        private readonly IConfig<RedisCachingConfig> _config;
        private readonly IRedisConnection _connection;
        private readonly ILogService _logService;
        
        private IDatabase _database;

        public string ConnectionName { get; }

        public RedisCachingProvider(string connectionName, IConfig<RedisCachingConfig> config, ILogService logService, IRedisConnection connection)
        {
            ConnectionName = connectionName;
            _config = config;
            _logService = logService;
            _connection = connection;
        }

        public async Task<bool> InvalidateAsync(string key)
        {
            try
            {
                return await _database.KeyExpireAsync(key, DateTime.Now.Subtract(TimeSpan.FromMinutes(1))).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logService.Error(
                    $"Failed to invalidate item with key '{key}' on Redis Cache Connection '{ConnectionName}': {ex.Message} ({ex.GetType()})",
                    ex);
                
                return false;
            }
        }

        public async Task<ICachedItem<TItem>> GetItemAsync<TItem>(string key, TimeSpan? timeout = null,
            CancellationToken? ct = null)
            where TItem : class
        {
            try
            {
                return JsonConvert.DeserializeObject<CachedItem<TItem>>(await _database.StringGetAsync(key).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                _logService.Error(
                    $"Failed to get item with key '{key}' from Redis Cache Connection '{ConnectionName}': {ex.Message} ({ex.GetType()})",
                    ex);

                return null;
            }
        }

        public async Task<bool> SetItemAsync<TItem>(string key, ICachedItem<TItem> item, TimeSpan? timeout = null,
            CancellationToken? ct = null)
            where TItem : class
        {
            try
            {
                return await _database.StringSetAsync(key, JsonConvert.SerializeObject(item),
                    TimeSpan.FromMilliseconds(item.ExtendedTimeToLiveMs.GetValueOrDefault(item.TimeToLiveMs))).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logService.Error(
                    $"Failed to set item with key '{key}' on Redis Cache Connection '{ConnectionName}': {ex.Message} ({ex.GetType()})",
                    ex);

                return false;
            }
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                _database = await _connection.GetDatabaseAsync().ConfigureAwait(false);
                _logService.Info($"Redis Cache Connection '{ConnectionName}' acquired database.");

                return true;
            }
            catch (Exception ex)
            {
                _logService.Error($"Failed to acquire Redis Caching Database for connection '{ConnectionName}': {ex.Message} ({ex.GetType()})",
                    ex);

                return false;
            }
        }
    }
}