using System;
using System.Threading.Tasks;
using XPike.Caching;

namespace XPike.Configuration.Caching
{
    public class CachingConfigurationPipe
        : ICachingConfigurationPipe
    {
        private readonly string _CACHE_KEY_FORMAT = $"{nameof(CachingConfigurationPipe)}.{{0}}";

        private readonly ICachingService _cachingService;
        private readonly ConfigurationCachingConfig _config;

        public CachingConfigurationPipe(ICachingService cachingService, IConfigManager<ConfigurationCachingConfig> config)
        {
            _cachingService = cachingService;
            
            // NOTE: We can't update this after load as that would cause a circular dependency if we're registered in the pipeline.
            _config = config.GetConfigOrDefault(new ConfigurationCachingConfig
            {
                Enabled = true,
                AllowExtendedTtlFailover = true,
                ConfigurationExtendedTtlSeconds = (int) TimeSpan.FromDays(1).TotalSeconds,
                CacheConnection = null,
                ConfigurationTtlSeconds = (int) TimeSpan.FromMinutes(15).TotalSeconds
            }).CurrentValue;
        }

        protected async Task<ConfigurationValue<T>> GetCachedValueAsync<T>(string key)
        {
            if (_config.Enabled)
            {
                try
                {
                    var value = await _cachingService.GetValueAsync<ConfigurationValue<T>>(_config.CacheConnection,
                        string.Format(_CACHE_KEY_FORMAT, key)).ConfigureAwait(false);

                    if (value != null)
                        return value;
                }
                catch (Exception)
                {
                }
            }

            return null;
        }

        protected async Task<ConfigurationValue<T>> GetExtendedCachedValueAsync<T>(string key)
        {
            if (_config.Enabled) 
            {
                try
                {
                    var item = await _cachingService.GetItemAsync<ConfigurationValue<T>>(_config.CacheConnection,
                        string.Format(_CACHE_KEY_FORMAT, key)).ConfigureAwait(false);

                    if (item?.Value != null && !(item?.IsExpired ?? true))
                        return item.Value;
                }
                catch (Exception)
                {
                }
            }

            return null;
        }

        protected async Task<bool> SetValueInCacheAsync<T>(string key, T value)
        {
            if (_config.Enabled)
            {
                try
                {
                    return await _cachingService.SetValueAsync<ConfigurationValue<T>>(_config.CacheConnection,
                        string.Format(_CACHE_KEY_FORMAT, key),
                        new ConfigurationValue<T>
                        {
                            Key = key,
                            Value = value
                        },
                        TimeSpan.FromSeconds(_config.ConfigurationTtlSeconds),
                        _config.AllowExtendedTtlFailover
                            ? TimeSpan.FromSeconds(_config.ConfigurationExtendedTtlSeconds)
                            : (TimeSpan?) null).ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }

            return false;
        }

        public string PipelineGet(string key, Func<string, string> next)
        {
            if (_config.Enabled)
            {
                try
                {
                    // Try to get from cache
                    var item = GetCachedValueAsync<string>(key).GetAwaiter().GetResult();
                    
                    // If we got from cache, just return -- we don't want to re-cache on each read.
                    if (item?.Value != null)
                        return item.Value;

                    // Try to get from underlying pipeline.
                    var value = next(key);

                    // If we got something, cache it and return.
                    if (value != null)
                    {
                        _ = Task.Run(async () => await SetValueInCacheAsync(key, value).ConfigureAwait(false));
                        return value;
                    }

                    // Try to get from cache using Extended TTL, if we're allowed to.
                    return _config.AllowExtendedTtlFailover
                        ? GetExtendedCachedValueAsync<string>(key).GetAwaiter().GetResult()?.Value
                        : null;
                }
                catch (Exception)
                {
                }
            }

            return next(key);
        }

        public async Task<string> PipelineGetAsync(string key, Func<string, Task<string>> next)
        {
            if (_config.Enabled)
            {
                try
                {
                    // Try to get from cache
                    var item = await GetCachedValueAsync<string>(key).ConfigureAwait(false);

                    // If we got from cache, just return -- we don't want to re-cache on each read.
                    if (item?.Value != null)
                        return item.Value;

                    // Try to get from underlying pipeline.
                    var value = await next(key).ConfigureAwait(false);

                    // If we got something, cache it and return.
                    if (value != null)
                    {
                        _ = Task.Run(async () => await SetValueInCacheAsync(key, value).ConfigureAwait(false));
                        return value;
                    }

                    // Try to get from cache using Extended TTL, if we're allowed to.
                    return _config.AllowExtendedTtlFailover
                        ? (await GetExtendedCachedValueAsync<string>(key).ConfigureAwait(false))?.Value
                        : null;
                }
                catch (Exception)
                {
                }
            }

            return await next(key).ConfigureAwait(false);
        }

        public T PipelineGet<T>(string key, Func<string, T> next)
        {
            if (_config.Enabled)
            {
                try
                {
                    // Try to get from cache
                    var item = GetCachedValueAsync<T>(key).GetAwaiter().GetResult();

                    // If we got from cache, just return -- we don't want to re-cache on each read.
                    if (item != null)
                        return item.Value;

                    // Try to get from underlying pipeline.
                    var value = next(key);

                    // If we got something, cache it and return.
                    if (value != null)
                    {
                        _ = Task.Run(async () => await SetValueInCacheAsync(key, value).ConfigureAwait(false));
                        return value;
                    }

                    // Try to get from cache using Extended TTL, if we're allowed to.
                    if (_config.AllowExtendedTtlFailover &&
                        (item = GetExtendedCachedValueAsync<T>(key).GetAwaiter().GetResult()) != null)
                        return item.Value;
                }
                catch (Exception)
                {
                }
            }

            return next(key);
        }

        public async Task<T> PipelineGetAsync<T>(string key, Func<string, Task<T>> next)
        {
            if (_config.Enabled)
            {
                try
                {
                    // Try to get from cache
                    var item = await GetCachedValueAsync<T>(key).ConfigureAwait(false);

                    // If we got from cache, just return -- we don't want to re-cache on each read.
                    if (item != null)
                        return item.Value;

                    // Try to get from underlying pipeline.
                    var value = await next(key).ConfigureAwait(false);

                    // If we got something, cache it and return.
                    if (value != null)
                    {
                        _ = Task.Run(async () => await SetValueInCacheAsync(key, value).ConfigureAwait(false));
                        return value;
                    }

                    // Try to get from cache using Extended TTL, if we're allowed to.
                    if (_config.AllowExtendedTtlFailover &&
                        (item = await GetExtendedCachedValueAsync<T>(key).ConfigureAwait(false)) != null)
                        return item.Value;
                }
                catch (Exception)
                {
                }
            }

            return await next(key).ConfigureAwait(false);
        }
    }
}