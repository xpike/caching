using System.Collections.Generic;

namespace XPike.Caching.Redis
{
    public class RedisCachingSettings
    {
        public IDictionary<string, RedisCachingConnectionSettings> Connections { get; set; }
    }
}