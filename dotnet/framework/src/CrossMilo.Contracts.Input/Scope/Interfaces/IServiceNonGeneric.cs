namespace Plate.CrossMilo.Contracts.Input.Scope;

/// <summary>
/// Non-generic scope service for backward compatibility with proxy service pattern.
/// Handles GameInputEvent (legacy, game-specific).
///
/// For new code, use IService&lt;TInputEvent&gt; instead.
/// </summary>
public interface IService
{
    /// <summary>
    /// Handle the input event.
    /// Returns true if handled (stops propagation).
    /// </summary>
    bool Handle(GameInputEvent inputEvent);

    /// <summary>
    /// If true, this scope captures ALL input events even if not handled.
    /// </summary>
    bool CaptureAll { get; }
}
