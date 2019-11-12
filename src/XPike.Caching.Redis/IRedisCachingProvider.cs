using System.Threading.Tasks;

namespace XPike.Caching.Redis
{
    public interface IRedisCachingProvider
        : ICachingProvider
    {
        string ConnectionName { get; }

        Task<bool> ConnectAsync();
    }
}