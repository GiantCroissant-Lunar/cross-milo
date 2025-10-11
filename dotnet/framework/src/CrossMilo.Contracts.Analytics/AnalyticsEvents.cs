using System;

namespace Plate.CrossMilo.Contracts.Analytics;

/// <summary>
/// Event published when analytics tracking state changes.
/// </summary>
public record AnalyticsTrackingStateChangedEvent(
    bool Enabled,
    string? Reason,
    DateTimeOffset Timestamp
)
{
    public AnalyticsTrackingStateChangedEvent(bool enabled, string? reason = null)
        : this(enabled, reason, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a user is identified.
/// </summary>
public record UserIdentifiedEvent(
    string UserId,
    string? Email,
    string? Name,
    DateTimeOffset Timestamp
)
{
    public UserIdentifiedEvent(string userId, string? email = null, string? name = null)
        : this(userId, email, name, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when an analytics event is tracked.
/// </summary>
public record AnalyticsEventTrackedEvent(
    string EventName,
    string? UserId,
    string? SessionId,
    DateTimeOffset Timestamp
)
{
    public AnalyticsEventTrackedEvent(string eventName, string? userId = null, string? sessionId = null)
        : this(eventName, userId, sessionId, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when analytics data is flushed to backend.
/// </summary>
public record AnalyticsFlushedEvent(
    int EventCount,
    string Backend,
    bool Success,
    DateTimeOffset Timestamp
)
{
    public AnalyticsFlushedEvent(int eventCount, string backend, bool success)
        : this(eventCount, backend, success, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when analytics session starts.
/// </summary>
public record AnalyticsSessionStartedEvent(
    string SessionId,
    string? UserId,
    DateTimeOffset Timestamp
)
{
    public AnalyticsSessionStartedEvent(string sessionId, string? userId = null)
        : this(sessionId, userId, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when analytics session ends.
/// </summary>
public record AnalyticsSessionEndedEvent(
    string SessionId,
    TimeSpan Duration,
    int EventsTracked,
    DateTimeOffset Timestamp
)
{
    public AnalyticsSessionEndedEvent(string sessionId, TimeSpan duration, int eventsTracked)
        : this(sessionId, duration, eventsTracked, DateTimeOffset.UtcNow)
    {
    }
}
