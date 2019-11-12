using XPike.IoC;

namespace XPike.Caching.Redis
{
    public class Package
        : IDependencyPackage
    {
        public void RegisterPackage(IDependencyCollection collection)
        {
            collection.LoadPackage(new XPike.Caching.Package());
            collection.LoadPackage(new XPike.Redis.Package());

            collection.RegisterSingleton<IRedisCachingConnectionProvider, RedisCachingConnectionProvider>();
        }
    }
}