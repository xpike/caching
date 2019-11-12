using System.Threading.Tasks;

namespace XPike.Caching.InMemory
{
    public class InMemoryCachingConnectionProvider
        : IInMemoryCachingConnectionProvider
    {
        private readonly IInMemoryCachingProvider _provider;

        public InMemoryCachingConnectionProvider(IInMemoryCachingProvider provider)
        {
            _provider = provider;
        }

        public Task<ICachingProvider> GetConnectionAsync(string connectionName) =>
            Task.FromResult(_provider as ICachingProvider);
    }
}