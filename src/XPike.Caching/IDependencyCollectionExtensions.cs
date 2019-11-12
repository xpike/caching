using XPike.IoC;

namespace XPike.Caching
{
    public static class IDependencyCollectionExtensions
    {
        public static IDependencyCollection AddXPikeCaching(this IDependencyCollection collection) =>
            collection.LoadPackage(new XPike.Caching.Package());
    }
}