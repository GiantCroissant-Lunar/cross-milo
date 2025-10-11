using System;

namespace Plate.CrossMilo.Contracts.Input;

/// <summary>
/// TEMPORARY COMPATIBILITY SHIM: Game input event type.
/// This is a temporary bridge to allow compilation during migration from CrossMilo.Contracts.Game.
/// 
/// TODO: Remove this once Input services are moved to game-specific code or refactored
/// to not depend on game-specific input types.
/// 
/// The real GameInputEvent is now in ConsoleDungeon.Contracts (game-specific).
/// This shim allows CrossMilo.Contracts.Input interfaces to compile without breaking changes.
/// </summary>
public record GameInputEvent(
    GameInputType Type,
    DateTimeOffset Timestamp
);

/// <summary>
/// TEMPORARY COMPATIBILITY SHIM: Game input types (platform-agnostic).
/// TODO: Remove this once Input services are refactored.
/// </summary>
public enum GameInputType
{
    // Movement
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,

    // Actions
    Attack,
    Use,
    Pickup,

    // UI
    ToggleMenu,
    ToggleInventory,

    // System
    Quit
}
