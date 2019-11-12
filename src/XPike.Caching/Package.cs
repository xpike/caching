using XPike.Caching.Hybrid;
using XPike.Caching.InMemory;
using XPike.Caching.NullInvalidation;
using XPike.Caching.NullProvider;
using XPike.IoC;

namespace XPike.Caching
{
    public class Package
        : IDependencyPackage
    {
        public void RegisterPackage(IDependencyCollection dependencyCollection)
        {
            dependencyCollection.LoadPackage(new XPike.Logging.Package());
            dependencyCollection.RegisterSingleton<ICachingService, CachingService>();

            dependencyCollection.RegisterSingleton<INullCacheInvalidationService, NullCacheInvalidationService>();
            dependencyCollection.RegisterSingleton<ICacheInvalidationService>(container =>
                container.ResolveDependency<INullCacheInvalidationService>());

            dependencyCollection.RegisterSingleton<INullCachingConnectionProvider, NullCachingConnectionProvider>();
            dependencyCollection.RegisterSingleton<INullCachingProvider, NullCachingProvider>();

            dependencyCollection.RegisterSingleton<IHybridCachingConnectionProvider, HybridCachingConnectionProvider>();

            dependencyCollection.RegisterSingleton<IInMemoryCachingConnectionProvider, InMemoryCachingConnectionProvider>();
            dependencyCollection.RegisterSingleton<IInMemoryCachingProvider, InMemoryCachingProvider>();
        }
    }
}