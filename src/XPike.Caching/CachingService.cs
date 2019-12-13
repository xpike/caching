using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XPike.Caching
{
    public class CachingService
        : ICachingService
    {
        private readonly ConcurrentDictionary<string, ICachingConnectionProvider> _providers = new ConcurrentDictionary<string, ICachingConnectionProvider>();
        private readonly ICacheInvalidationService _invalidationService;

        public CachingService(ICacheInvalidationService invalidationService)
        {
            _invalidationService = invalidationService;
        }

        public void AddProvider(string connection, ICachingConnectionProvider provider) =>
            _providers[connection ?? "default"] = provider;

        public async Task<bool> InvalidateAsync(string connectionName, string key)
        {
            var connection = await GetProviderAsync(connectionName ?? "default").ConfigureAwait(false);

            return (await Task.WhenAll(connection.InvalidateAsync(key),
                    _invalidationService.SendInvalidationAsync(connectionName ?? "default", key)).ConfigureAwait(false))
                .All(x => x);
        }

        protected Task<ICachingProvider> GetProviderAsync(string connectionName)
        {
            if (_providers.TryGetValue(connectionName ?? "default", out var provider))
                return provider.GetConnectionAsync(connectionName ?? "default");

            return _providers["default"].GetConnectionAsync("default");
        }

        public async Task<ICachedItem<TItem>> GetItemAsync<TItem>(string connectionName, string key, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class
        {
            var connection = await GetProviderAsync(connectionName).ConfigureAwait(false);
            return await connection.GetItemAsync<TItem>(key, timeout, ct).ConfigureAwait(false);
        }

        public async Task<bool> SetItemAsync<TItem>(string connectionName, string key, ICachedItem<TItem> item, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class
        {
            var connection = await GetProviderAsync(connectionName).ConfigureAwait(false);

            return (await Task.WhenAll(connection.SetItemAsync(key, item, timeout, ct),
                    _invalidationService.SendInvalidationAsync(connectionName ?? "default", key)).ConfigureAwait(false))
                .All(x => x);
        }

        public async Task<TItem> GetValueAsync<TItem>(string connectionName, string key, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class
        {
            var item = await GetItemAsync<TItem>(connectionName, key, timeout, ct).ConfigureAwait(false);
            var expiration = item.Timestamp.AddMilliseconds(item.ExtendedTimeToLiveMs.GetValueOrDefault(item.TimeToLiveMs));

            if (expiration > DateTime.UtcNow)
                return null;

            return item.Value;
        }

        public Task<bool> SetValueAsync<TItem>(string connectionName,
            string key,
            TItem item,
            TimeSpan ttl,
            TimeSpan? extendedTtl = null,
            TimeSpan? timeout = null,
            CancellationToken? ct = null)
            where TItem : class =>
            SetItemAsync(connectionName,
                key,
                new CachedItem<TItem>
                {
                    ExtendedTimeToLiveMs = (int?) extendedTtl?.TotalMilliseconds,
                    TimeToLiveMs = (int) ttl.TotalMilliseconds,
                    Value = item,
                    Timestamp = DateTime.UtcNow,
                    Key = key,
                    Connection = connectionName
                },
                timeout,
                ct);
    }
}