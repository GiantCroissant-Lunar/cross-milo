using System;

namespace Plate.CrossMilo.Contracts.Resilience;

/// <summary>
/// Event published when a retry attempt is made.
/// </summary>
public record RetryAttemptEvent(
    string StrategyName,
    int AttemptNumber,
    int MaxAttempts,
    Exception Exception,
    DateTimeOffset Timestamp
)
{
    public RetryAttemptEvent(string strategyName, int attemptNumber, int maxAttempts, Exception exception)
        : this(strategyName, attemptNumber, maxAttempts, exception, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when all retry attempts are exhausted.
/// </summary>
public record RetryExhaustedEvent(
    string StrategyName,
    int TotalAttempts,
    Exception FinalException,
    DateTimeOffset Timestamp
)
{
    public RetryExhaustedEvent(string strategyName, int totalAttempts, Exception finalException)
        : this(strategyName, totalAttempts, finalException, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when circuit breaker state changes.
/// </summary>
public record CircuitBreakerStateChangedEvent(
    string StrategyName,
    CircuitState OldState,
    CircuitState NewState,
    string? Reason,
    DateTimeOffset Timestamp
)
{
    public CircuitBreakerStateChangedEvent(string strategyName, CircuitState oldState, CircuitState newState, string? reason = null)
        : this(strategyName, oldState, newState, reason, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when an operation times out.
/// </summary>
public record OperationTimeoutEvent(
    string StrategyName,
    string OperationName,
    TimeSpan Timeout,
    TimeSpan ActualDuration,
    DateTimeOffset Timestamp
)
{
    public OperationTimeoutEvent(string strategyName, string operationName, TimeSpan timeout, TimeSpan actualDuration)
        : this(strategyName, operationName, timeout, actualDuration, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a fallback is executed.
/// </summary>
public record FallbackExecutedEvent(
    string StrategyName,
    string FallbackName,
    Exception TriggeringException,
    DateTimeOffset Timestamp
)
{
    public FallbackExecutedEvent(string strategyName, string fallbackName, Exception triggeringException)
        : this(strategyName, fallbackName, triggeringException, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when rate limit is exceeded.
/// </summary>
public record RateLimitExceededEvent(
    string StrategyName,
    int CurrentRate,
    int MaxRate,
    TimeSpan Window,
    DateTimeOffset Timestamp
)
{
    public RateLimitExceededEvent(string strategyName, int currentRate, int maxRate, TimeSpan window)
        : this(strategyName, currentRate, maxRate, window, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a resilience operation succeeds after failures.
/// </summary>
public record ResilienceOperationSucceededEvent(
    string StrategyName,
    string OperationName,
    int PreviousFailureCount,
    TimeSpan Duration,
    DateTimeOffset Timestamp
)
{
    public ResilienceOperationSucceededEvent(string strategyName, string operationName, int previousFailureCount, TimeSpan duration)
        : this(strategyName, operationName, previousFailureCount, duration, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a resilience operation fails.
/// </summary>
public record ResilienceOperationFailedEvent(
    string StrategyName,
    string OperationName,
    Exception Exception,
    TimeSpan Duration,
    DateTimeOffset Timestamp
)
{
    public ResilienceOperationFailedEvent(string strategyName, string operationName, Exception exception, TimeSpan duration)
        : this(strategyName, operationName, exception, duration, DateTimeOffset.UtcNow)
    {
    }
}
