using System;

namespace Plate.CrossMilo.Contracts.Input;

/// <summary>
/// Event published when raw input is received from platform.
/// </summary>
public record RawInputReceivedEvent(
    RawKeyEvent KeyEvent,
    DateTimeOffset Timestamp
)
{
    public RawInputReceivedEvent(RawKeyEvent keyEvent)
        : this(keyEvent, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when input is mapped to an action.
/// </summary>
public record InputActionMappedEvent(
    string ActionName,
    RawKeyEvent KeyEvent,
    string? Context,
    DateTimeOffset Timestamp
)
{
    public InputActionMappedEvent(string actionName, RawKeyEvent keyEvent, string? context = null)
        : this(actionName, keyEvent, context, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when input scope changes.
/// </summary>
public record InputScopeChangedEvent(
    string? OldScope,
    string NewScope,
    DateTimeOffset Timestamp
)
{
    public InputScopeChangedEvent(string? oldScope, string newScope)
        : this(oldScope, newScope, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when input mappings are reloaded.
/// </summary>
public record InputMappingsReloadedEvent(
    int MappingCount,
    string Source,
    DateTimeOffset Timestamp
)
{
    public InputMappingsReloadedEvent(int mappingCount, string source)
        : this(mappingCount, source, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when an input action is triggered.
/// </summary>
public record InputActionTriggeredEvent(
    string ActionName,
    string? Context,
    object? Payload,
    DateTimeOffset Timestamp
)
{
    public InputActionTriggeredEvent(string actionName, string? context = null, object? payload = null)
        : this(actionName, context, payload, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when input is blocked or filtered.
/// </summary>
public record InputBlockedEvent(
    RawKeyEvent KeyEvent,
    string Reason,
    DateTimeOffset Timestamp
)
{
    public InputBlockedEvent(RawKeyEvent keyEvent, string reason)
        : this(keyEvent, reason, DateTimeOffset.UtcNow)
    {
    }
}
