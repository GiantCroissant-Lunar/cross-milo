using Microsoft.Extensions.Hosting;

namespace Plate.CrossMilo.Contracts.Hosting.App;

/// <summary>
/// Base interface for all Winged Bean applications.
/// Provides standard lifecycle management across platforms.
/// </summary>
public interface IService : IHostedService
{
    /// <summary>
    /// Application name (for logging, diagnostics).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Current application state.
    /// </summary>
    AppState State { get; }

    /// <summary>
    /// Fired when application state changes.
    /// </summary>
    event EventHandler<AppStateChangedEventArgs>? StateChanged;
}

/// <summary>
/// Application lifecycle states.
/// </summary>
public enum AppState
{
    NotStarted,
    Starting,
    Running,
    Stopping,
    Stopped,
    Faulted
}

public class AppStateChangedEventArgs : EventArgs
{
    public AppState PreviousState { get; set; }
    public AppState NewState { get; set; }
    public Exception? Error { get; set; }
}
