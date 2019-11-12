using System;

namespace XPike.Caching
{
    public abstract class CachedItem
        : ICachedItem
    {
        public string Connection { get; set; }

        public string Key { get; set; }

        public DateTime Timestamp { get; set; }

        public int TimeToLiveMs { get; set; }

        public int? ExtendedTimeToLiveMs { get; set; }

        public bool IsStale => DateTime.UtcNow > Timestamp.AddMilliseconds(TimeToLiveMs);

        public bool IsExpired => DateTime.UtcNow > Timestamp.AddMilliseconds(ExtendedTimeToLiveMs.GetValueOrDefault(TimeToLiveMs));
    }

    public class CachedItem<TItem>
        : CachedItem,
          ICachedItem<TItem>
        where TItem : class
    {
        public TItem Value { get; set; }
    }
}