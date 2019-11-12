using System;

namespace XPike.Caching.InMemory
{
    public interface IInMemoryCachingProvider
        : ICachingProvider,
          IDisposable
    {
    }
}