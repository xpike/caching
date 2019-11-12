using System;
using System.Threading;
using System.Threading.Tasks;

namespace XPike.Caching
{
    public interface ICachingService
    {
        void AddProvider(string connection, ICachingConnectionProvider provider);

        Task<bool> InvalidateAsync(string connectionName, string key);

        Task<ICachedItem<TItem>> GetItemAsync<TItem>(string connectionName, string key, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class;

        Task<bool> SetItemAsync<TItem>(string connectionName, string key, ICachedItem<TItem> item, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class;

        Task<TItem> GetValueAsync<TItem>(string connectionName, string key, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class;

        Task<bool> SetValueAsync<TItem>(string connectionName, 
            string key,
            TItem item,
            TimeSpan ttl,
            TimeSpan? extendedTtl = null,
            TimeSpan? timeout = null,
            CancellationToken? ct = null)
            where TItem : class;
    }
}