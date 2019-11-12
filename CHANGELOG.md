# Change Log

## [1.0.0]

Inital release.

Basic caching support for xPike.

- Only supports simple Get/Set of a serialized object by cache key right now.
- Supports multiple named connections, each with a different configuration.
- Supports caching in local memory through the InMemoryCacheProvider.
- Supports N-tier cache configurations through the HybridCacheProvider.
- Supports distributed invalidation of local caches through a pub/sub mechanism.
- Supports an Extended TTL concept to distinguish "stale" from "expired" data.
    - Allows stale data to be used during a transient outage of a datasource.

Basic Redis caching support for xPike.

- Connections are defined and configured through application settings.
- Named caches can be mapped as desired to one or more named Redis connections.
- Uses XPike.Redis for connection management
- XPike.Redis uses StackExchange.Redis as its communication library.
- The current implementation of XPike.Redis serializes objects in JSON as a Redis String.
 