namespace XPike.Caching.Hybrid
{
    public interface IHybridCachingConnectionProvider
        : ICachingConnectionProvider
    {
        void AddConnectionProvider(string connectionName, ICachingConnectionProvider provider);
    }
}