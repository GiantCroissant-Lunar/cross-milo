using System;

namespace Plate.CrossMilo.Contracts.Scene;

/// <summary>
/// TEMPORARY COMPATIBILITY SHIM: Entity snapshot for rendering.
/// This is a temporary bridge to allow compilation during migration from CrossMilo.Contracts.Game.
/// 
/// TODO: Remove this once Scene services are refactored to not depend on game-specific types,
/// or move Scene contracts to game-specific code.
/// 
/// The real EntitySnapshot is now in ConsoleDungeon.Contracts (game-specific).
/// This shim allows CrossMilo.Contracts.Scene interfaces to compile without breaking changes.
/// </summary>
public record EntitySnapshot(
    Guid Id,
    Position Position,
    char Symbol,
    ConsoleColor ForegroundColor,
    ConsoleColor BackgroundColor,
    int RenderLayer
);

/// <summary>
/// TEMPORARY COMPATIBILITY SHIM: Position type for entity snapshots.
/// TODO: Remove this once Scene services are refactored.
/// </summary>
public record Position(int X, int Y, int Floor);
