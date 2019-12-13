using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XPike.Caching.Hybrid
{
    public class HybridCachingProvider
        : IHybridCachingProvider
    {
        private readonly IList<ICachingConnectionProvider> _providers;
        private readonly string _connectionName;

        public HybridCachingProvider(string connectionName, IList<ICachingConnectionProvider> providers)
        {
            _providers = providers;
            _connectionName = connectionName;
        }

        protected async Task<IList<TResult>> ParallelExecuteAsync<TResult>(
            Func<ICachingProvider, Task<TResult>> asyncOperation) =>
            await Task.WhenAll(_providers.Select(async x =>
                await asyncOperation(await x.GetConnectionAsync(_connectionName).ConfigureAwait(false))
                    .ConfigureAwait(false))).ConfigureAwait(false);

        protected async Task<IList<ICachingProvider>> GetProvidersAsync() =>
            (await Task.WhenAll(_providers.Select(x => x.GetConnectionAsync(_connectionName))).ConfigureAwait(false))
            .ToList();

        public async Task<bool> InvalidateAsync(string key) =>
            (await ParallelExecuteAsync<bool>(provider => provider.InvalidateAsync(key)).ConfigureAwait(false))
            .All(x => x);

        public async Task<ICachedItem<TItem>> GetItemAsync<TItem>(string key, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class
        {
            var previousProviders = new List<ICachingProvider>();
            var providers = await GetProvidersAsync().ConfigureAwait(false);

            ICachedItem<TItem> item = null;
            ICachedItem<TItem> bestMatch = null;

            foreach (var provider in providers)
            {
                var found = await provider.GetItemAsync<TItem>(key, timeout, ct).ConfigureAwait(false);

                if (!found.IsStale)
                {
                    item = found;
                    break;
                }

                if (!found.IsExpired)
                    bestMatch = found;

                previousProviders.Add(provider);
            }

            // NOTE: Intentional fire-and-forget.  We don't want to wait for cache "percolation" to complete.
#pragma warning disable 4014
            if (item != null)
                _ = Task.Run(async () =>
                    await Task.WhenAll(previousProviders.Select(x => x.SetItemAsync(key, item, timeout, ct)))
                        .ConfigureAwait(false));
#pragma warning restore 4014

            return item ?? bestMatch;
        }

        public async Task<bool> SetItemAsync<TItem>(string key, ICachedItem<TItem> item, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class =>
            (await ParallelExecuteAsync(provider => provider.SetItemAsync(key, item, timeout, ct)).ConfigureAwait(false))
            .All(x => x);
    }
}