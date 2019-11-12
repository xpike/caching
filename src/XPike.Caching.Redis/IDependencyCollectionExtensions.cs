using XPike.IoC;

namespace XPike.Caching.Redis
{
    public static class IDependencyCollectionExtensions
    {
        public static IDependencyCollection AddXPikeRedisCaching(this IDependencyCollection collection) =>
            collection.LoadPackage(new XPike.Caching.Redis.Package());
    }
}