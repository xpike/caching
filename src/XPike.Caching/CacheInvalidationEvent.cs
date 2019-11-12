using System;

namespace XPike.Caching
{
    public class CacheInvalidationEvent
    {
        public string Connection { get; set; }

        public string Key { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid InstanceId { get; set; }
    }
}