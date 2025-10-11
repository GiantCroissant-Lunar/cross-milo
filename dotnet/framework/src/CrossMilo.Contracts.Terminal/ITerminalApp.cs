using System;
using System.Threading;
using System.Threading.Tasks;

namespace Plate.CrossMilo.Contracts.Terminal;

/// <summary>
/// Terminal-specific UI application contract.
/// Extends IUIApp with terminal-specific capabilities (ANSI, PTY, etc.).
/// </summary>
public interface ITerminalApp : Plate.CrossMilo.Contracts.UI.App.IService
{
    // Terminal-specific operations
    Task SendRawInputAsync(byte[] data, CancellationToken ct = default);
    Task SetCursorPositionAsync(int x, int y, CancellationToken ct = default);
    Task WriteAnsiAsync(string ansiSequence, CancellationToken ct = default);

    // Terminal-specific events
    event EventHandler<TerminalOutputEventArgs>? OutputReceived;
    event EventHandler<TerminalExitEventArgs>? Exited;
}

/// <summary>
/// Event args for terminal output
/// </summary>
public class TerminalOutputEventArgs : EventArgs
{
    /// <summary>Output data</summary>
    public byte[] Data { get; set; } = Array.Empty<byte>();

    /// <summary>Timestamp</summary>
    public DateTimeOffset Timestamp { get; set; }
}

/// <summary>
/// Event args for terminal exit
/// </summary>
public class TerminalExitEventArgs : EventArgs
{
    /// <summary>Exit code</summary>
    public int ExitCode { get; set; }

    /// <summary>Exit timestamp</summary>
    public DateTimeOffset Timestamp { get; set; }
}
