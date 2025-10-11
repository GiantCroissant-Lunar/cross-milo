// TEMPORARY: GameInputEvent moved to CrossMilo.Contracts.Input as compatibility shim
// TODO: Refactor Input services to not depend on game-specific types

namespace Plate.CrossMilo.Contracts.Input.Scope;

/// <summary>
/// A handler for a scope of input.
/// Scopes are pushed/popped to handle modal dialogs, menus, gameplay, etc.
/// </summary>
public interface IService
{
    /// <summary>
    /// Handle the input event.
    /// Returns true if handled (stops propagation).
    /// Returns false if not handled (may propagate to lower scope if CaptureAll is false).
    /// </summary>
    bool Handle(GameInputEvent inputEvent);

    /// <summary>
    /// If true, this scope captures ALL input events even if not handled.
    /// Used for modal dialogs to prevent input leaking to gameplay.
    /// </summary>
    bool CaptureAll { get; }
}
