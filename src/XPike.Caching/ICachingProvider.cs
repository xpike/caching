using System;
using System.Threading;
using System.Threading.Tasks;

namespace XPike.Caching
{
    public interface ICachingProvider
    {
        Task<bool> InvalidateAsync(string key);

        Task<ICachedItem<TItem>> GetItemAsync<TItem>(string key, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class;

        Task<bool> SetItemAsync<TItem>(string key, ICachedItem<TItem> item, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class;
    }
}