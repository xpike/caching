using System;
using System.Threading;
using System.Threading.Tasks;

namespace XPike.Caching.NullProvider
{
    public class NullCachingProvider
        : INullCachingProvider
    {
        public Task<bool> InvalidateAsync(string key) =>
            Task.FromResult(true);

        public Task<ICachedItem<TItem>> GetItemAsync<TItem>(string key, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class =>
            Task.FromResult<ICachedItem<TItem>>(null);

        public Task<bool> SetItemAsync<TItem>(string key, ICachedItem<TItem> item, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class =>
            Task.FromResult(true);
    }
}