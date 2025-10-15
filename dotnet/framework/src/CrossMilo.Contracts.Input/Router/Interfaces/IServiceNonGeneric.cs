using System;
using Plate.CrossMilo.Contracts.Input.Scope;

namespace Plate.CrossMilo.Contracts.Input.Router;

/// <summary>
/// Non-generic router service for backward compatibility with proxy service pattern.
/// Routes GameInputEvent (legacy, game-specific).
///
/// For new code, use IService&lt;TInputEvent&gt; instead.
/// </summary>
public interface IService
{
    /// <summary>
    /// Push a new input scope onto the stack.
    /// Returns IDisposable that pops the scope when disposed.
    /// </summary>
    IDisposable PushScope(Scope.IService scope);

    /// <summary>
    /// Dispatch a game input event to the current top scope.
    /// </summary>
    void Dispatch(GameInputEvent inputEvent);

    /// <summary>
    /// Get the current top scope (active input handler).
    /// </summary>
    Scope.IService? Top { get; }
}
