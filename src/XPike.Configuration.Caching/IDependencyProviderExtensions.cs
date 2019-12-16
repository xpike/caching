using XPike.IoC;

namespace XPike.Configuration.Caching
{
    public static class IDependencyProviderExtensions
    {
        public static IDependencyProvider UseXPikeConfigurationCaching(this IDependencyProvider provider)
        {
            provider.ResolveDependency<IConfigurationService>()
                .AddToPipeline(provider.ResolveDependency<ICachingConfigurationPipe>());

            return provider;
        }
    }
}