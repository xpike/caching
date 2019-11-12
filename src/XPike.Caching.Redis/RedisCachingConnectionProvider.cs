using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using XPike.Logging;
using XPike.Redis;
using XPike.Settings;

namespace XPike.Caching.Redis
{
    public class RedisCachingConnectionProvider
        : IRedisCachingConnectionProvider
    {
        public const string DEFAULT_CONNECTION_NAME = "default";

        private readonly ConcurrentDictionary<string, IRedisCachingProvider> _connections = new ConcurrentDictionary<string, IRedisCachingProvider>();

        private readonly IRedisConnectionProvider _connectionProvider;
        private readonly ISettings<RedisCachingSettings> _settings;
        private readonly ILogService _logService;

        public RedisCachingConnectionProvider(ISettings<RedisCachingSettings> settings, ILogService logService, IRedisConnectionProvider connectionProvider)
        {
            _settings = settings;
            _logService = logService;
            _connectionProvider = connectionProvider;
        }

        public async Task<ICachingProvider> GetConnectionAsync(string connectionName)
        {
            if (connectionName == null)
                return await GetConnectionAsync(DEFAULT_CONNECTION_NAME);

            if (_connections.TryGetValue(connectionName, out var connection))
                return connection;

            if (!_settings.Value.Connections.ContainsKey(connectionName))
            {
                if (connectionName == DEFAULT_CONNECTION_NAME)
                    throw new InvalidOperationException("No configuration found for default Redis caching connection!");

                _logService.Warn($"No configuration found for Redis Caching connection '{connectionName}' - using default connection.");
                
                connection = await GetConnectionAsync(DEFAULT_CONNECTION_NAME) as IRedisCachingProvider;
            }
            else
            {
                connection = new RedisCachingProvider(connectionName,
                    _settings,
                    _logService,
                    await _connectionProvider.GetConnectionAsync(_settings.Value
                        .Connections[connectionName]
                        .RedisConnectionName));

                if (!await connection.ConnectAsync())
                    _logService.Warn($"Failed to connect to Redis Cache '{connectionName}': Provider returned false!");
            }

            _connections[connectionName] = connection;
            
            return connection;
        }
    }
}