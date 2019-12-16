using XPike.IoC;

namespace XPike.Configuration.Caching
{
    public static class IDependencyCollectionExtensions
    {
        public static IDependencyCollection AddXPikeConfigurationCaching(this IDependencyCollection collection) =>
            collection.LoadPackage(new XPike.Configuration.Caching.Package());
    }
}