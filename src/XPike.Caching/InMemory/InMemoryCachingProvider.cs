using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XPike.Caching.InMemory
{
    public class InMemoryCachingProvider
        : IInMemoryCachingProvider
    {
        private readonly ConcurrentDictionary<string, ICachedItem> _items = new ConcurrentDictionary<string, ICachedItem>();
        private readonly ICacheInvalidationService _invalidationService;
        private bool _isDisposed = false;

        public InMemoryCachingProvider(ICacheInvalidationService invalidationService)
        {
            _invalidationService = invalidationService;
            _invalidationService.AddInvalidationHandler(invalidationEvent => InvalidateAsync(invalidationEvent.Key));

            _ = Task.Run(async () => await EnforceExpirationsAsync().ConfigureAwait(false));
        }

        public Task<bool> InvalidateAsync(string key)
        {
            _items.TryRemove(key, out _);
            return Task.FromResult(true);
        }

        public Task<ICachedItem<TItem>> GetItemAsync<TItem>(string key, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class =>
            Task.FromResult(_items[key] as ICachedItem<TItem>);

        public Task<bool> SetItemAsync<TItem>(string key, ICachedItem<TItem> item, TimeSpan? timeout = null, CancellationToken? ct = null)
            where TItem : class
        {
            _items[key] = item;
            return Task.FromResult(true);
        }

        private async Task EnforceExpirationsAsync()
        {
            while (!_isDisposed)
            {
                var remove = _items.Where(x => x.Value.IsExpired).Select(x => x.Key).ToList();
                await Task.WhenAll(remove.Select(InvalidateAsync)).ConfigureAwait(false);
                await Task.Delay(15000).ConfigureAwait(false);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isDisposed = true;
                _items.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}