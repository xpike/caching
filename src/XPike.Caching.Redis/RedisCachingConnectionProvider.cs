using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using XPike.Configuration;
using XPike.Logging;
using XPike.Redis;

namespace XPike.Caching.Redis
{
    public class RedisCachingConnectionProvider
        : IRedisCachingConnectionProvider
    {
        public const string DEFAULT_CONNECTION_NAME = "default";

        private readonly ConcurrentDictionary<string, IRedisCachingProvider> _connections = new ConcurrentDictionary<string, IRedisCachingProvider>();

        private readonly IRedisConnectionProvider _connectionProvider;
        private readonly IConfig<RedisCachingConfig> _config;
        private readonly ILogService _logService;

        public RedisCachingConnectionProvider(IConfig<RedisCachingConfig> config, ILogService logService, IRedisConnectionProvider connectionProvider)
        {
            _config = config;
            _logService = logService;
            _connectionProvider = connectionProvider;
        }

        public async Task<ICachingProvider> GetConnectionAsync(string connectionName)
        {
            if (connectionName == null)
                return await GetConnectionAsync(DEFAULT_CONNECTION_NAME).ConfigureAwait(false);

            if (_connections.TryGetValue(connectionName, out var connection))
                return connection;

            if (!_config.CurrentValue.Connections.ContainsKey(connectionName))
            {
                if (connectionName == DEFAULT_CONNECTION_NAME)
                    throw new InvalidOperationException("No configuration found for default Redis caching connection!");

                _logService.Warn($"No configuration found for Redis Caching connection '{connectionName}' - using default connection.");
                
                connection = await GetConnectionAsync(DEFAULT_CONNECTION_NAME).ConfigureAwait(false) as IRedisCachingProvider;
            }
            else
            {
                connection = new RedisCachingProvider(connectionName,
                    _config,
                    _logService,
                    await _connectionProvider.GetConnectionAsync(_config.CurrentValue
                        .Connections[connectionName]
                        .RedisConnectionName).ConfigureAwait(false));

                if (!await connection.ConnectAsync().ConfigureAwait(false))
                    _logService.Warn($"Failed to connect to Redis Cache '{connectionName}': Provider returned false!");
            }

            _connections[connectionName] = connection;
            
            return connection;
        }
    }
}