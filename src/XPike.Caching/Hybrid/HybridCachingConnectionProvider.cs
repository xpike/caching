using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XPike.Caching.Hybrid
{
    public class HybridCachingConnectionProvider
        : IHybridCachingConnectionProvider
    {
        private readonly ConcurrentDictionary<string, List<ICachingConnectionProvider>> _providerMap = new ConcurrentDictionary<string, List<ICachingConnectionProvider>>();
        private readonly ConcurrentDictionary<string, IHybridCachingProvider> _connections = new ConcurrentDictionary<string, IHybridCachingProvider>();

        public void AddConnectionProvider(string connectionName, ICachingConnectionProvider provider)
        {
            if (!_providerMap.ContainsKey(connectionName ?? "default"))
                _providerMap[connectionName ?? "default"] = new List<ICachingConnectionProvider>();

            _providerMap[connectionName ?? "default"].Add(provider);
        }

        public Task<ICachingProvider> GetConnectionAsync(string connectionName)
        {
            if (!_connections.ContainsKey(connectionName ?? "default"))
                _connections[connectionName ?? "default"] = new HybridCachingProvider(connectionName ?? "default", _providerMap[connectionName ?? "default"]);

            return Task.FromResult(_connections[connectionName ?? "default"] as ICachingProvider);
        }
    }
}