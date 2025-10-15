namespace Plate.CrossMilo.Contracts.Input.Mapper;

/// <summary>
/// Maps raw key events into high-level application-specific input events.
/// Framework-agnostic: implementations handle platform-specific quirks
/// (CSI sequences, SS3, ESC disambiguation, etc.)
/// </summary>
/// <typeparam name="TInputEvent">The type of input event to produce</typeparam>
public interface IService<TInputEvent>
    where TInputEvent : class
{
    /// <summary>
    /// Map a raw key into an application-specific input event or null if not recognized/pending.
    /// Implementations may buffer incomplete sequences (e.g., ESC [ A)
    /// or use short timers to disambiguate standalone ESC vs CSI sequences.
    /// </summary>
    TInputEvent? Map(RawKeyEvent rawEvent);

    /// <summary>
    /// Reset mapper state (clear buffered sequences, timers).
    /// Called when focus lost or context changed.
    /// </summary>
    void Reset();
}
