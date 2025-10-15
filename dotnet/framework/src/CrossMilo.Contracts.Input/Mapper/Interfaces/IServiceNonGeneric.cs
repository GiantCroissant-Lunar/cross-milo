namespace Plate.CrossMilo.Contracts.Input.Mapper;

/// <summary>
/// Non-generic mapper service for backward compatibility with proxy service pattern.
/// Maps raw key events into GameInputEvent (legacy, game-specific).
///
/// For new code, use IService&lt;TInputEvent&gt; instead.
/// </summary>
public interface IService
{
    /// <summary>
    /// Map a raw key into a GameInputEvent or null if not recognized/pending.
    /// </summary>
    GameInputEvent? Map(RawKeyEvent rawEvent);

    /// <summary>
    /// Reset mapper state (clear buffered sequences, timers).
    /// </summary>
    void Reset();
}
