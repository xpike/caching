using System.Threading.Tasks;

namespace XPike.Caching.NullProvider
{
    public class NullCachingConnectionProvider
        : INullCachingConnectionProvider
    {
        public Task<ICachingProvider> GetConnectionAsync(string connectionName) =>
            Task.FromResult(new NullCachingProvider() as ICachingProvider);
    }
}