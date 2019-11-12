using System.Threading.Tasks;

namespace XPike.Caching.NullInvalidation
{
    public class NullCacheInvalidationService
        : CacheInvalidationServiceBase,
          INullCacheInvalidationService
    {
        protected override Task<bool> SendInvalidationAsync(CacheInvalidationEvent invalidationEvent) =>
            Task.FromResult(true);
    }
}