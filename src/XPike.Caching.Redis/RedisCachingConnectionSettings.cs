namespace XPike.Caching.Redis
{
    public class RedisCachingConnectionSettings
    {
        public string RedisConnectionName { get; set; }

        public bool Enabled { get; set; }

        public int DefaultGetTimeoutMs { get; set; }

        public int DefaultSetTimeoutMs { get; set; }
    }
}