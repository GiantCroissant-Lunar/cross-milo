# Event Bus Integration Guide

## Overview

This document describes how to use the event-driven architecture with the EventBus for inter-plugin communication in the cross-milo/plugin-manoi/winged-bean ecosystem.

## Architecture Layers

### Tier 1: cross-milo (Contracts)
- **Purpose**: Define service interfaces and event contracts
- **Contains**: Event definitions (records), service interfaces, data models
- **Example**: `EntitySpawnedEvent`, `ConfigChangedEvent`, `IService` interfaces

### Tier 2: plugin-manoi (Infrastructure)
- **Purpose**: Plugin loading and event routing infrastructure
- **Contains**: `IEventBus`, `EventBus` implementation, plugin lifecycle
- **Registered in**: `HostBootstrap.RegisterHostServices()` as singleton

### Tier 3+: winged-bean plugins (Implementations)
- **Purpose**: Implement services and publish/subscribe to events
- **Contains**: Service implementations that use EventBus for communication

## Event Naming Convention

All events follow this pattern:

```csharp
public record {Subject}{Action}Event(
    // Required parameters
    string Id,
    // Optional parameters
    string? Context,
    // Timestamp (always last)
    DateTimeOffset Timestamp
)
{
    // Convenience constructor without timestamp
    public {Subject}{Action}Event(string id, string? context = null)
        : this(id, context, DateTimeOffset.UtcNow)
    {
    }
}
```

**Pattern**: `{Subject}{Action}Event`
- **Subject**: What the event is about (Entity, Config, Scene, etc.)
- **Action**: What happened (Spawned, Changed, Loaded, etc.)
- **Event**: Always suffix with "Event"

### Examples:
- ✅ `EntitySpawnedEvent` - Entity was spawned
- ✅ `ConfigChangedEvent` - Configuration changed
- ✅ `SceneLoadedEvent` - Scene was loaded
- ✅ `UserIdentifiedEvent` - User was identified
- ❌ `SpawnEntity` - Missing "Event" suffix, wrong tense
- ❌ `EntitySpawn` - Missing "Event" suffix

## Event Categories

### Game Events (`CrossMilo.Contracts.Game`)
- `EntitySpawnedEvent` - New entity created
- `EntityMovedEvent` - Entity changed position
- `EntityDiedEvent` - Entity removed from game
- `CombatEvent` - Combat interaction occurred
- `GameStateChangedEvent` - Game state transition
- `ItemCollectedEvent` - Item picked up
- `LevelUpEvent` - Player leveled up

### Config Events (`CrossMilo.Contracts.Config`)
- `ConfigChangedEvent` - Configuration value changed
- `ConfigReloadedEvent` - Configuration reloaded from source
- `ConfigValidationFailedEvent` - Invalid configuration detected
- `ConfigSavedEvent` - Configuration persisted

### Scene Events (`CrossMilo.Contracts.Scene`)
- `SceneLoadedEvent` - Scene finished loading
- `SceneUnloadedEvent` - Scene was unloaded
- `SceneTransitionStartedEvent` - Scene transition began
- `SceneTransitionCompletedEvent` - Scene transition finished
- `SceneShutdownRequestedEvent` - Scene shutdown initiated
- `ScenePausedEvent` - Scene paused
- `SceneResumedEvent` - Scene resumed

### Analytics Events (`CrossMilo.Contracts.Analytics`)
- `AnalyticsTrackingStateChangedEvent` - Tracking enabled/disabled
- `UserIdentifiedEvent` - User identity set
- `AnalyticsEventTrackedEvent` - Event tracked
- `AnalyticsFlushedEvent` - Data sent to backend
- `AnalyticsSessionStartedEvent` - Session began
- `AnalyticsSessionEndedEvent` - Session ended

### Diagnostics Events (`CrossMilo.Contracts.Diagnostics`)
- `ErrorCapturedEvent` - Error/exception captured
- `PerformanceMetricRecordedEvent` - Performance data recorded
- `BreadcrumbAddedEvent` - Diagnostic breadcrumb added
- `DiagnosticsDataSentEvent` - Data sent to backend
- `HealthCheckPerformedEvent` - Health check executed
- `DiagnosticsCaptureStateChangedEvent` - Capture enabled/disabled

### Input Events (`CrossMilo.Contracts.Input`)
- `RawInputReceivedEvent` - Raw input from platform
- `InputActionMappedEvent` - Input mapped to action
- `InputScopeChangedEvent` - Input context changed
- `InputMappingsReloadedEvent` - Mappings reloaded
- `InputActionTriggeredEvent` - Action triggered
- `InputBlockedEvent` - Input filtered/blocked

### Resource Events (`CrossMilo.Contracts.Resource`)
- `ResourceLoadedEvent` - Resource loaded successfully
- `ResourceLoadFailedEvent` - Resource load failed
- `ResourceUnloadedEvent` - Resource freed from memory
- `ResourceCacheClearedEvent` - Cache cleared
- `NuGetPackageDownloadedEvent` - NuGet package retrieved
- `ResourceDependencyResolvedEvent` - Dependency resolved

### Resilience Events (`CrossMilo.Contracts.Resilience`)
- `RetryAttemptEvent` - Retry initiated
- `RetryExhaustedEvent` - All retries failed
- `CircuitBreakerStateChangedEvent` - Circuit state changed
- `OperationTimeoutEvent` - Operation timed out
- `FallbackExecutedEvent` - Fallback triggered
- `RateLimitExceededEvent` - Rate limit hit
- `ResilienceOperationSucceededEvent` - Operation succeeded after failures
- `ResilienceOperationFailedEvent` - Operation failed

## Usage Patterns

### Publishing Events

**In a plugin service:**

```csharp
using Plate.CrossMilo.Contracts.Game;
using Plate.PluginManoi.Core;

public class DungeonGameService
{
    private readonly IEventBus _eventBus;

    public DungeonGameService(IRegistry registry)
    {
        _eventBus = registry.Get<IEventBus>();
    }

    public async Task SpawnEnemyAsync(Guid entityId, Position position)
    {
        // Do the work
        var enemy = CreateEnemy(entityId, position);

        // Publish event to notify other plugins
        await _eventBus.PublishAsync(new EntitySpawnedEvent(
            entityId,
            EntityType.Enemy,
            position
        ));
    }
}
```

### Subscribing to Events

**In a plugin's activation:**

```csharp
using Plate.CrossMilo.Contracts.Game;
using Plate.CrossMilo.Contracts.Analytics.Services;
using Plate.PluginManoi.Core;
using Plate.PluginManoi.Contracts;

public class AnalyticsPlugin : IPlugin
{
    public async Task OnActivateAsync(IRegistry registry, CancellationToken ct)
    {
        var eventBus = registry.Get<IEventBus>();
        var analyticsService = registry.Get<IService>(); // Analytics service

        // Subscribe to game events
        eventBus.Subscribe<EntitySpawnedEvent>(async evt =>
        {
            await analyticsService.Track("EntitySpawned", new
            {
                evt.EntityId,
                EntityType = evt.Type.ToString(),
                evt.Position
            });
        });

        eventBus.Subscribe<CombatEvent>(async evt =>
        {
            await analyticsService.Track("Combat", new
            {
                evt.AttackerId,
                evt.DefenderId,
                evt.Damage,
                evt.DefenderKilled
            });
        });
    }
}
```

### Cross-Plugin Communication Pattern

**Scenario**: When a player levels up, track it in analytics and show diagnostics

```csharp
// In DungeonGameService (publisher)
await _eventBus.PublishAsync(new LevelUpEvent(playerId, newLevel));

// In AnalyticsPlugin (subscriber)
eventBus.Subscribe<LevelUpEvent>(async evt =>
{
    await _analyticsService.Track("PlayerLevelUp", new
    {
        PlayerId = evt.EntityId,
        Level = evt.NewLevel
    });
});

// In DiagnosticsPlugin (subscriber)
eventBus.Subscribe<LevelUpEvent>(async evt =>
{
    _logger.LogInformation("Player {PlayerId} reached level {Level}",
        evt.EntityId, evt.NewLevel);
});
```

## Best Practices

### 1. Use Records for Events
Events should be immutable `record` types, not classes:

```csharp
// ✅ Good
public record EntitySpawnedEvent(Guid EntityId, EntityType Type, Position Position);

// ❌ Bad
public class EntitySpawnedEvent
{
    public Guid EntityId { get; set; }
    public EntityType Type { get; set; }
}
```

### 2. Include Timestamps
Always include a timestamp, with a convenience constructor:

```csharp
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
```

### 3. Async Handlers
Always use async handlers to avoid blocking:

```csharp
// ✅ Good
eventBus.Subscribe<EntitySpawnedEvent>(async evt =>
{
    await _analyticsService.Track("EntitySpawned", new { evt.EntityId });
});

// ❌ Bad - blocks event bus
eventBus.Subscribe<EntitySpawnedEvent>(evt =>
{
    _analyticsService.Track("EntitySpawned", new { evt.EntityId }).Wait();
    return Task.CompletedTask;
});
```

### 4. Error Handling in Subscribers
Handle errors to prevent one subscriber from breaking others:

```csharp
eventBus.Subscribe<EntitySpawnedEvent>(async evt =>
{
    try
    {
        await _analyticsService.Track("EntitySpawned", new { evt.EntityId });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to track entity spawn");
        // Don't rethrow - let other subscribers continue
    }
});
```

### 5. Unsubscribe When Done
If you need to unsubscribe (rarely needed since plugins live for app lifetime):

```csharp
// EventBus currently doesn't support unsubscribe
// Design assumes subscribers live for application lifetime
// If needed, implement IDisposable pattern in your handler
```

### 6. Event Granularity
Publish specific events rather than generic ones:

```csharp
// ✅ Good - specific events
await _eventBus.PublishAsync(new EntitySpawnedEvent(id, type, pos));
await _eventBus.PublishAsync(new EntityDiedEvent(id, type));

// ❌ Bad - generic event
await _eventBus.PublishAsync(new EntityEvent("spawn", id, type, pos));
```

### 7. Don't Overuse Events
Use events for cross-cutting concerns, not everything:

**Use events for:**
- ✅ Analytics tracking
- ✅ Diagnostics logging
- ✅ State synchronization across plugins
- ✅ Notification of state changes

**Don't use events for:**
- ❌ Direct service calls (use `IRegistry.Get<T>()` instead)
- ❌ Request/response patterns (use direct method calls)
- ❌ Tightly-coupled operations within same plugin

## Migration from EventArgs

Legacy code using `EventArgs`:

```csharp
// Old pattern (EventArgs)
public event EventHandler<ConfigChangedEventArgs>? ConfigChanged;
ConfigChanged?.Invoke(this, new ConfigChangedEventArgs { Key = key, ... });

// New pattern (EventBus)
await _eventBus.PublishAsync(new ConfigChangedEvent(key, oldValue, newValue));
```

Both can coexist during migration, but prefer EventBus for new code.

## Testing Events

### Unit Testing Publishers

```csharp
[Fact]
public async Task SpawnEnemy_PublishesEntitySpawnedEvent()
{
    // Arrange
    var eventBus = new EventBus();
    EntitySpawnedEvent? captured = null;
    eventBus.Subscribe<EntitySpawnedEvent>(evt =>
    {
        captured = evt;
        return Task.CompletedTask;
    });

    var service = new DungeonGameService(eventBus);

    // Act
    await service.SpawnEnemyAsync(enemyId, position);

    // Assert
    Assert.NotNull(captured);
    Assert.Equal(enemyId, captured.EntityId);
    Assert.Equal(EntityType.Enemy, captured.Type);
}
```

### Unit Testing Subscribers

```csharp
[Fact]
public async Task OnEntitySpawned_TracksAnalytics()
{
    // Arrange
    var analyticsService = Substitute.For<IService>();
    var plugin = new AnalyticsPlugin(analyticsService);
    var eventBus = new EventBus();
    await plugin.ActivateAsync(registry, CancellationToken.None);

    // Act
    await eventBus.PublishAsync(new EntitySpawnedEvent(id, type, pos));

    // Assert
    await analyticsService.Received(1).Track("EntitySpawned", Arg.Any<object>());
}
```

## Performance Considerations

1. **EventBus is in-memory** - No persistence, no durability
2. **All handlers run sequentially** - Use `Task.WhenAll` in EventBus for parallel execution
3. **No dead-letter queue** - Failed handlers don't retry
4. **No ordering guarantees** - Handlers may run in any order

For high-performance scenarios, consider:
- Batching events before publishing
- Using dedicated message queues (RabbitMQ, Azure Service Bus) for critical events
- Implementing async fire-and-forget patterns

## Summary

✅ **Events belong in cross-milo contracts** - Shared across all implementations
✅ **EventBus lives in plugin-manoi** - Infrastructure for routing
✅ **Plugins publish and subscribe** - Loosely coupled communication
✅ **Use modern record syntax** - Immutable, concise events
✅ **Include timestamps** - Track when events occurred
✅ **Handle errors gracefully** - Don't break other subscribers

---

**Related Files:**
- Event definitions: `CrossMilo.Contracts.*/Events.cs`
- EventBus interface: `PluginManoi.Core/HostBootstrap.cs:375`
- EventBus implementation: `PluginManoi.Core/HostBootstrap.cs:395`
- Registration: `PluginManoi.Core/HostBootstrap.cs:209`
