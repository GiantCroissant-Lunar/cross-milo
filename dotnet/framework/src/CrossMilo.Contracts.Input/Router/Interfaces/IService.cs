using System;
using Plate.CrossMilo.Contracts.Input.Scope;

namespace Plate.CrossMilo.Contracts.Input.Router;

/// <summary>
/// Scoped routing model for input events.
/// New scopes are pushed when dialogs/menus open and popped on close.
/// Enables modal input capture without leaking to gameplay.
/// </summary>
/// <typeparam name="TInputEvent">The type of input event to route</typeparam>
public interface IService<TInputEvent>
    where TInputEvent : class
{
    /// <summary>
    /// Push a new input scope onto the stack.
    /// Returns IDisposable that pops the scope when disposed.
    /// </summary>
    IDisposable PushScope(Scope.IService<TInputEvent> scope);

    /// <summary>
    /// Dispatch an input event to the current top scope.
    /// If top scope doesn't handle it and CaptureAll is false,
    /// optionally propagates to lower scopes (default: no propagation).
    /// </summary>
    void Dispatch(TInputEvent inputEvent);

    /// <summary>
    /// Get the current top scope (active input handler).
    /// Null if no scopes pushed.
    /// </summary>
    Scope.IService<TInputEvent>? Top { get; }
}
