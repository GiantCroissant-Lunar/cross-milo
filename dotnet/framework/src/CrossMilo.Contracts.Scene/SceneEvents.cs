using System;

namespace Plate.CrossMilo.Contracts.Scene;

/// <summary>
/// Event published when a scene is loaded.
/// </summary>
public record SceneLoadedEvent(
    string SceneId,
    string SceneName,
    TimeSpan LoadDuration,
    DateTimeOffset Timestamp
)
{
    public SceneLoadedEvent(string sceneId, string sceneName, TimeSpan loadDuration)
        : this(sceneId, sceneName, loadDuration, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a scene is unloaded.
/// </summary>
public record SceneUnloadedEvent(
    string SceneId,
    string SceneName,
    DateTimeOffset Timestamp
)
{
    public SceneUnloadedEvent(string sceneId, string sceneName)
        : this(sceneId, sceneName, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a scene transition begins.
/// </summary>
public record SceneTransitionStartedEvent(
    string FromSceneId,
    string ToSceneId,
    string TransitionType,
    DateTimeOffset Timestamp
)
{
    public SceneTransitionStartedEvent(string fromSceneId, string toSceneId, string transitionType)
        : this(fromSceneId, toSceneId, transitionType, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when a scene transition completes.
/// </summary>
public record SceneTransitionCompletedEvent(
    string FromSceneId,
    string ToSceneId,
    TimeSpan TransitionDuration,
    DateTimeOffset Timestamp
)
{
    public SceneTransitionCompletedEvent(string fromSceneId, string toSceneId, TimeSpan transitionDuration)
        : this(fromSceneId, toSceneId, transitionDuration, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when scene shutdown is requested.
/// </summary>
public record SceneShutdownRequestedEvent(
    string SceneId,
    ShutdownReason Reason,
    DateTimeOffset Timestamp
)
{
    public SceneShutdownRequestedEvent(string sceneId, ShutdownReason reason)
        : this(sceneId, reason, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when scene is paused.
/// </summary>
public record ScenePausedEvent(
    string SceneId,
    string Reason,
    DateTimeOffset Timestamp
)
{
    public ScenePausedEvent(string sceneId, string reason)
        : this(sceneId, reason, DateTimeOffset.UtcNow)
    {
    }
}

/// <summary>
/// Event published when scene is resumed.
/// </summary>
public record SceneResumedEvent(
    string SceneId,
    TimeSpan PauseDuration,
    DateTimeOffset Timestamp
)
{
    public SceneResumedEvent(string sceneId, TimeSpan pauseDuration)
        : this(sceneId, pauseDuration, DateTimeOffset.UtcNow)
    {
    }
}
