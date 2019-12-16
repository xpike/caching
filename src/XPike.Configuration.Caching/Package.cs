using XPike.IoC;

namespace XPike.Configuration.Caching
{
    public class Package
        : IDependencyPackage
    {
        public void RegisterPackage(IDependencyCollection dependencyCollection)
        {
            dependencyCollection.LoadPackage(new XPike.Caching.Package());

            dependencyCollection.RegisterSingleton<ICachingConfigurationPipe, CachingConfigurationPipe>();
        }
    }
}