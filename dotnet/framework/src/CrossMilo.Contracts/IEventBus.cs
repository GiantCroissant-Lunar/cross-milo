using System;
using System.Threading.Tasks;

namespace Plate.CrossMilo.Contracts;

/// <summary>
/// Event bus for inter-plugin communication using publish-subscribe pattern.
/// </summary>
/// <remarks>
/// <para>
/// The event bus enables loosely-coupled communication between plugins without
/// requiring direct dependencies. Publishers emit events without knowing who will
/// consume them, and subscribers react to events without knowing who published them.
/// </para>
/// <para>
/// <strong>Usage Pattern:</strong>
/// </para>
/// <list type="bullet">
///   <item><description>Publishers: <c>await eventBus.PublishAsync(new EntitySpawnedEvent(...))</c></description></item>
///   <item><description>Subscribers: <c>eventBus.Subscribe&lt;EntitySpawnedEvent&gt;(async evt => { ... })</c></description></item>
/// </list>
/// <para>
/// <strong>Architecture:</strong>
/// </para>
/// <list type="bullet">
///   <item><description>Event contracts (records): Defined in <c>Plate.CrossMilo.Contracts.*</c></description></item>
///   <item><description>Event bus interface: <c>Plate.CrossMilo.Contracts.IEventBus</c> (this file)</description></item>
///   <item><description>Event bus implementation: <c>Plate.PluginManoi.Core.EventBus</c></description></item>
///   <item><description>Event publishers/subscribers: Plugin implementations</description></item>
/// </list>
/// <para>
/// <strong>Example:</strong>
/// </para>
/// <code>
/// // Publisher (in DungeonGameService)
/// await _eventBus.PublishAsync(new EntitySpawnedEvent(entityId, EntityType.Enemy, position));
///
/// // Subscriber (in AnalyticsPlugin.OnActivateAsync)
/// eventBus.Subscribe&lt;EntitySpawnedEvent&gt;(async evt =>
/// {
///     await _analyticsService.Track("EntitySpawned", new { evt.EntityId, evt.Type });
/// });
/// </code>
/// </remarks>
public interface IEventBus
{
    /// <summary>
    /// Publish an event to all subscribers.
    /// </summary>
    /// <typeparam name="T">Event type (must be a reference type)</typeparam>
    /// <param name="eventData">Event data to publish</param>
    /// <returns>Task that completes when all subscribers have been notified</returns>
    /// <remarks>
    /// <para>
    /// All registered subscribers for type <typeparamref name="T"/> will be notified
    /// asynchronously. Handlers are executed sequentially (not in parallel) to maintain
    /// predictable ordering.
    /// </para>
    /// <para>
    /// <strong>Error Handling:</strong> If a subscriber throws an exception, it should
    /// be caught by the implementation to prevent breaking other subscribers.
    /// </para>
    /// <para>
    /// <strong>Performance Note:</strong> This is an in-memory event bus with no
    /// persistence or durability guarantees. For critical events requiring guaranteed
    /// delivery, use a dedicated message queue system.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// await eventBus.PublishAsync(new ConfigChangedEvent("theme", "dark", "light"));
    /// </code>
    /// </example>
    Task PublishAsync<T>(T eventData) where T : class;

    /// <summary>
    /// Subscribe to events of a specific type.
    /// </summary>
    /// <typeparam name="T">Event type to subscribe to (must be a reference type)</typeparam>
    /// <param name="handler">Async handler function to invoke when event is published</param>
    /// <remarks>
    /// <para>
    /// Subscribers typically register during plugin activation and remain subscribed
    /// for the lifetime of the application. There is currently no unsubscribe mechanism
    /// as plugins are expected to live for the full application lifetime.
    /// </para>
    /// <para>
    /// <strong>Thread Safety:</strong> Handlers should be thread-safe as events may be
    /// published from different threads.
    /// </para>
    /// <para>
    /// <strong>Best Practices:</strong>
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Keep handlers lightweight and fast</description></item>
    ///   <item><description>Use async/await properly (don't block)</description></item>
    ///   <item><description>Handle exceptions within the handler</description></item>
    ///   <item><description>Don't throw exceptions that would break other subscribers</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// eventBus.Subscribe&lt;ConfigChangedEvent&gt;(async evt =>
    /// {
    ///     try
    ///     {
    ///         _logger.LogInformation("Config changed: {Key} = {NewValue}", evt.Key, evt.NewValue);
    ///         await Task.CompletedTask; // Placeholder for async work
    ///     }
    ///     catch (Exception ex)
    ///     {
    ///         _logger.LogError(ex, "Failed to handle config change");
    ///     }
    /// });
    /// </code>
    /// </example>
    void Subscribe<T>(Func<T, Task> handler) where T : class;
}
