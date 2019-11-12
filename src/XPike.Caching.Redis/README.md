# XPike.Caching.Redis

[![Build Status](https://dev.azure.com/xpike/xpike/_apis/build/status/xpike.caching?branchName=master)](https://dev.azure.com/xpike/xpike/_build/latest?definitionId=9&branchName=master)
![Nuget](https://img.shields.io/nuget/v/XPike.Caching.Redis)

Provides Redis caching support for xPike.

## Overview

xPike Redis Caching is the recommended approach for caching object data in xPike on a Redis server or cluster.

- Connections are defined and configured through application settings.
- Named caches can be mapped as desired to one or more named Redis connections.
- Uses XPike.Redis for connection management
- XPike.Redis uses StackExchange.Redis as its communication library.
- The current implementation of XPike.Redis serializes objects in JSON as a Redis String.

## Exported Services

### Singletons

- **`IRedisCachingConnectionProvider`**  
  **=> `RedisCachingConnectionProvider`**

## Usage

**In ASP.NET Core:**

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddXPikeDependencyInjection()
            .AddXPikeRedisCaching();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseXPikeDependencyInjection()
       .UseXPikeCacheProvider<IHybridCachingConnectionProvider>(null)
       .AddXPikeHybridCacheProvider<IInMemoryCachingConnectionProvider>(null)
       .AddXPikeHybridCacheProvider<IRedisCachingConnectionProvider>(null);
}
```

## Dependencies

- `StackExchange.Redis`
- `XPike.IoC`
- `XPike.Redis`
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
