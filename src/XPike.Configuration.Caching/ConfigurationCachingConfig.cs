namespace XPike.Configuration.Caching
{
    public class ConfigurationCachingConfig
    {
        public string CacheConnection { get; set; }

        public int ConfigurationTtlSeconds { get; set; }

        public int ConfigurationExtendedTtlSeconds { get; set; }

        public bool Enabled { get; set; }

        public bool AllowExtendedTtlFailover { get; set; }
    }
}