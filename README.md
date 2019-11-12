# XPike.Caching

[![Build Status](https://dev.azure.com/xpike/xpike/_apis/build/status/xpike.caching?branchName=master)](https://dev.azure.com/xpike/xpike/_build/latest?definitionId=9&branchName=master)
![Nuget](https://img.shields.io/nuget/v/XPike.Caching)

Provides caching support for xPike.

## Overview

xPike Caching is the recommended approach for caching object data in xPike.

- Only supports simple Get/Set of a serialized object by cache key right now.
- Supports multiple named connections, each with a different configuration.
- Supports caching in local memory through the InMemoryCacheProvider.
- Supports N-tier cache configurations through the HybridCacheProvider.
- Supports distributed invalidation of local caches through a pub/sub mechanism.
- Supports an Extended TTL concept to distinguish "stale" from "expired" data.
    - Allows stale data to be used during a transient outage of a datasource.

## Exported Services

### Singletons

- **`ICachingService`**  
  **=> `CachingService`**

- **`ICacheInvalidationService`**  
  **=> `NullCacheInvalidationService`**

- **`IInMemoryCachingConnectionProvider`**  
  **=> `InMemoryCachingConnectionProvider`**

- **`IHybridCachingConnectionProvider`**  
  **=> `HybridCachingConnectionProvider`**

## Usage

**In ASP.NET Core:**

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddXPikeDependencyInjection()
            .AddXPikeCaching();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseXPikeDependencyInjection()
       .UseXPikeCacheProvider<IHybridCachingConnectionProvider>(null)
       .AddXPikeHybridCacheProvider<IInMemoryCachingConnectionProvider>(null);
}
```

## Dependencies

- `XPike.Logging`
  - `XPike.Settings`
    - `XPike.Configuration`
      - `XPike.IoC`
  - `XPike.IoC`

## Building and Testing

Building from source and running unit tests requires a Windows machine with:

* .Net Core 3.0 SDK
* .Net Framework 4.6.1 Developer Pack

## Issues

Issues are tracked on [GitHub](https://github.com/xpike/xpike-caching/issues). Anyone is welcome to file a bug,
an enhancement request, or ask a general question. We ask that bug reports include:

1. A detailed description of the problem
2. Steps to reproduce
3. Expected results
4. Actual results
5. Version of the package xPike
6. Version of the .Net runtime

## Contributing

See our [contributing guidelines](https://github.com/xpike/documentation/blob/master/docfx_project/articles/contributing.md)
in our documentation for information on how to contribute to xPike.

## License

xPike is licensed under the [MIT License](LICENSE).
