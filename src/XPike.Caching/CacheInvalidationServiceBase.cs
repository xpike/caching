using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XPike.Caching
{
    public abstract class CacheInvalidationServiceBase
        : ICacheInvalidationService
    {
        private static readonly Guid _instanceId = Guid.NewGuid();

        private readonly List<Func<CacheInvalidationEvent, Task<bool>>> _handlers = new List<Func<CacheInvalidationEvent, Task<bool>>>();

        protected abstract Task<bool> SendInvalidationAsync(CacheInvalidationEvent invalidationEvent);

        public Task<bool> SendInvalidationAsync(string connection, string key) =>
            SendInvalidationAsync(new CacheInvalidationEvent
            {
                Connection = connection,
                Timestamp = DateTime.UtcNow,
                Key = key,
                InstanceId = _instanceId
            });

        public async Task<bool> HandleInvalidationAsync(CacheInvalidationEvent invalidationEvent)
        {
            if (invalidationEvent.InstanceId == _instanceId)
                return true;

            return (await Task.WhenAll(_handlers.Select(x => x(invalidationEvent)))).All(x => x);
        }

        public void AddInvalidationHandler(Func<CacheInvalidationEvent, Task<bool>> asyncHandler) =>
            _handlers.Add(asyncHandler);
    }
}