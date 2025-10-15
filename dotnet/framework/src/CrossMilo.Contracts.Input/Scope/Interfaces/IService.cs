namespace Plate.CrossMilo.Contracts.Input.Scope;

/// <summary>
/// A handler for a scope of input.
/// Scopes are pushed/popped to handle modal dialogs, menus, gameplay, etc.
/// </summary>
/// <typeparam name="TInputEvent">The type of input event to handle</typeparam>
public interface IService<TInputEvent>
    where TInputEvent : class
{
    /// <summary>
    /// Handle the input event.
    /// Returns true if handled (stops propagation).
    /// Returns false if not handled (may propagate to lower scope if CaptureAll is false).
    /// </summary>
    bool Handle(TInputEvent inputEvent);

    /// <summary>
    /// If true, this scope captures ALL input events even if not handled.
    /// Used for modal dialogs to prevent input leaking to gameplay.
    /// </summary>
    bool CaptureAll { get; }
}
