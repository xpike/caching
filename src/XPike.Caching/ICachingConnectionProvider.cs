using System.Threading.Tasks;

namespace XPike.Caching
{
    public interface ICachingConnectionProvider
    {
        Task<ICachingProvider> GetConnectionAsync(string connectionName);
    }
}