using System;

namespace XPike.Caching
{
    public interface ICachedItem
    {
        string Connection { get; }

        string Key { get; }

        DateTime Timestamp { get; }

        int TimeToLiveMs { get; }

        int? ExtendedTimeToLiveMs { get; }

        bool IsStale { get; }

        bool IsExpired { get; }
    }

    public interface ICachedItem<TItem>
        : ICachedItem
        where TItem : class
    {
        TItem Value { get; }
    }
}