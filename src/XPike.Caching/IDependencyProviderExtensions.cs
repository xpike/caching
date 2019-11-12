using XPike.Caching.Hybrid;
using XPike.IoC;

namespace XPike.Caching
{
    public static class IDependencyProviderExtensions
    {
        public static IDependencyProvider AddXPikeHybridCacheProvider<TProvider>(this IDependencyProvider provider, string connectionName)
            where TProvider : ICachingConnectionProvider
        {
            provider.ResolveDependency<IHybridCachingConnectionProvider>()
                .AddConnectionProvider(connectionName,
                    provider.ResolveDependency(typeof(TProvider)) as ICachingConnectionProvider);

            return provider;
        }

        public static IDependencyProvider UseXPikeCacheProvider<TProvider>(this IDependencyProvider provider, string connectionName)
            where TProvider : ICachingConnectionProvider
        {
            provider.ResolveDependency<ICachingService>()
                .AddProvider(connectionName,
                    provider.ResolveDependency(typeof(TProvider)) as ICachingConnectionProvider);
            
            return provider;
        }
    }
}