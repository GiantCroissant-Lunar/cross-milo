using System;

namespace Plate.CrossMilo.Contracts.Resource;

/// <summary>
/// Event published when a resource is loaded.
/// </summary>
public record ResourceLoadedEvent(
    string ResourceId,
    string ResourceType,
    string Source,
    TimeSpan LoadDuration,
    DateTimeOffset Timestamp
)
{
    public ResourceLoadedEvent(string resourceId, string resourceType, string source, TimeSpan loadDuration)
        : this(resourceId, resourceType, source, loadDuration, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a resource fails to load.
/// </summary>
public record ResourceLoadFailedEvent(
    string ResourceId,
    string ResourceType,
    string Source,
    string ErrorMessage,
    Exception? Exception,
    DateTimeOffset Timestamp
)
{
    public ResourceLoadFailedEvent(string resourceId, string resourceType, string source, string errorMessage, Exception? exception = null)
        : this(resourceId, resourceType, source, errorMessage, exception, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a resource is unloaded from memory.
/// </summary>
public record ResourceUnloadedEvent(
    string ResourceId,
    string ResourceType,
    string Reason,
    DateTimeOffset Timestamp
)
{
    public ResourceUnloadedEvent(string resourceId, string resourceType, string reason)
        : this(resourceId, resourceType, reason, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when resource cache is cleared.
/// </summary>
public record ResourceCacheClearedEvent(
    int ResourcesCleared,
    long BytesFreed,
    DateTimeOffset Timestamp
)
{
    public ResourceCacheClearedEvent(int resourcesCleared, long bytesFreed)
        : this(resourcesCleared, bytesFreed, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a NuGet package is downloaded.
/// </summary>
public record NuGetPackageDownloadedEvent(
    string PackageId,
    string Version,
    string Source,
    TimeSpan DownloadDuration,
    DateTimeOffset Timestamp
)
{
    public NuGetPackageDownloadedEvent(string packageId, string version, string source, TimeSpan downloadDuration)
        : this(packageId, version, source, downloadDuration, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a resource dependency is resolved.
/// </summary>
public record ResourceDependencyResolvedEvent(
    string ResourceId,
    string DependencyId,
    string DependencyType,
    DateTimeOffset Timestamp
)
{
    public ResourceDependencyResolvedEvent(string resourceId, string dependencyId, string dependencyType)
        : this(resourceId, dependencyId, dependencyType, DateTimeOffset.UtcNow)
    {
    }
}
