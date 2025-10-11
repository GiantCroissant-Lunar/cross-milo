using System;

namespace Plate.CrossMilo.Contracts.Config;

/// <summary>
/// Event published when a configuration value changes.
/// </summary>
public record ConfigChangedEvent(
    string Key,
    string? OldValue,
    string? NewValue,
    DateTimeOffset Timestamp
)
{
    public ConfigChangedEvent(string key, string? oldValue, string? newValue)
        : this(key, oldValue, newValue, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when configuration is reloaded from source.
/// </summary>
public record ConfigReloadedEvent(
    string Source,
    int ChangedKeysCount,
    DateTimeOffset Timestamp
)
{
    public ConfigReloadedEvent(string source, int changedKeysCount)
        : this(source, changedKeysCount, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when configuration validation fails.
/// </summary>
public record ConfigValidationFailedEvent(
    string Key,
    string AttemptedValue,
    string ValidationError,
    DateTimeOffset Timestamp
)
{
    public ConfigValidationFailedEvent(string key, string attemptedValue, string validationError)
        : this(key, attemptedValue, validationError, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when configuration is saved to persistent storage.
/// </summary>
public record ConfigSavedEvent(
    string Destination,
    int SavedKeysCount,
    bool Success,
    DateTimeOffset Timestamp
)
{
    public ConfigSavedEvent(string destination, int savedKeysCount, bool success)
        : this(destination, savedKeysCount, success, DateTimeOffset.UtcNow)
    {
    }
}
