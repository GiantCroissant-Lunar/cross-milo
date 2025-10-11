using System;

namespace Plate.CrossMilo.Contracts.Diagnostics;

/// <summary>
/// Event published when an error is captured by diagnostics.
/// </summary>
public record ErrorCapturedEvent(
    string ErrorId,
    string Message,
    string? StackTrace,
    Exception? Exception,
    string Level,
    DateTimeOffset Timestamp
)
{
    public ErrorCapturedEvent(string errorId, string message, string? stackTrace, Exception? exception, string level)
        : this(errorId, message, stackTrace, exception, level, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a performance metric is recorded.
/// </summary>
public record PerformanceMetricRecordedEvent(
    string MetricName,
    double Value,
    string Unit,
    DateTimeOffset Timestamp
)
{
    public PerformanceMetricRecordedEvent(string metricName, double value, string unit)
        : this(metricName, value, unit, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a breadcrumb is added.
/// </summary>
public record BreadcrumbAddedEvent(
    string Message,
    string Category,
    string Level,
    DateTimeOffset Timestamp
)
{
    public BreadcrumbAddedEvent(string message, string category, string level)
        : this(message, category, level, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when diagnostics data is sent to backend.
/// </summary>
public record DiagnosticsDataSentEvent(
    string Backend,
    int EventCount,
    bool Success,
    DateTimeOffset Timestamp
)
{
    public DiagnosticsDataSentEvent(string backend, int eventCount, bool success)
        : this(backend, eventCount, success, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a health check is performed.
/// </summary>
public record HealthCheckPerformedEvent(
    string CheckName,
    string Status,
    TimeSpan Duration,
    string? Message,
    DateTimeOffset Timestamp
)
{
    public HealthCheckPerformedEvent(string checkName, string status, TimeSpan duration, string? message = null)
        : this(checkName, status, duration, message, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when diagnostics capture is enabled or disabled.
/// </summary>
public record DiagnosticsCaptureStateChangedEvent(
    bool Enabled,
    string? Reason,
    DateTimeOffset Timestamp
)
{
    public DiagnosticsCaptureStateChangedEvent(bool enabled, string? reason = null)
        : this(enabled, reason, DateTimeOffset.UtcNow)
    {
    }
}
