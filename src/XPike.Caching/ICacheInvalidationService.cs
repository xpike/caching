using System;
using System.Threading.Tasks;

namespace XPike.Caching
{
    public interface ICacheInvalidationService
    {
        Task<bool> SendInvalidationAsync(string connection, string key);

        Task<bool> HandleInvalidationAsync(CacheInvalidationEvent invalidationEvent);

        void AddInvalidationHandler(Func<CacheInvalidationEvent, Task<bool>> asyncHandler);
    }
}