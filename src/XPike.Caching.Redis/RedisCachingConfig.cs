using System.Collections.Generic;

namespace XPike.Caching.Redis
{
    public class RedisCachingConfig
    {
        public IDictionary<string, RedisCachingConnectionSettings> Connections { get; set; }
    }
}