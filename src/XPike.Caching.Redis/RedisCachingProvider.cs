using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;
using XPike.Logging;
using XPike.Redis;
using XPike.Settings;
using Exception = System.Exception;

namespace XPike.Caching.Redis
{
    public class RedisCachingProvider
        : IRedisCachingProvider
    {
        private readonly ISettings<RedisCachingSettings> _settings;
        private readonly IRedisConnection _connection;
        private readonly ILogService _logService;
        
        private IDatabase _database;

        public string ConnectionName { get; }

        public RedisCachingProvider(string connectionName, ISettings<RedisCachingSettings> settings, ILogService logService, IRedisConnection connection)
        {
            ConnectionName = connectionName;
            _settings = settings;
            _logService = logService;
            _connection = connection;
        }

        public async Task<bool> InvalidateAsync(string key)
        {
            try
            {
                return await _database.KeyExpireAsync(key, DateTime.Now.Subtract(TimeSpan.FromMinutes(1)));
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
                return JsonConvert.DeserializeObject<CachedItem<TItem>>(await _database.StringGetAsync(key));
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
                    TimeSpan.FromMilliseconds(item.ExtendedTimeToLiveMs.GetValueOrDefault(item.TimeToLiveMs)));
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
                _database = await _connection.GetDatabaseAsync();
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