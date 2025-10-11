# ⚠️ DEPRECATED

**This project is deprecated and should not be used in new code.**

## Migration Date
2025-10-11

## Reason
Game contracts have been moved to be game-specific. Framework-level game contracts created unnecessary coupling between the framework and specific game implementations.

## Migration Path

### Before (Old)
```csharp
using Plate.CrossMilo.Contracts.Game.Dungeon;

// Using framework game contract
public class MyDungeonGame : IService
{
    // Implementation
}
```

### After (New)
```csharp
using ConsoleDungeon.Contracts;

// Using game-specific contract
public class MyDungeonGame : IDungeonService
{
    // Implementation
}
```

## Replacement

- **Old**: `Plate.CrossMilo.Contracts.Game.Dungeon.IService`
- **New**: `ConsoleDungeon.Contracts.IDungeonService` (game-specific)

## Status

- ❌ **Removed from build-config.json** - Will not be built in CI/CD
- ❌ **Removed from Input/Scene dependencies** - No longer referenced
- ⚠️ **Still in solution** - For backward compatibility only
- 🔒 **No new features** - This project is frozen

## See Also

- Migration document: `/Users/apprenticegc/Work/lunar-horse/personal-work/yokan-projects/winged-bean/development/dotnet/console/MIGRATION_GAME_CONTRACTS.md`
- Game contracts: `/Users/apprenticegc/Work/lunar-horse/personal-work/yokan-projects/winged-bean/development/dotnet/console/src/game/ConsoleDungeon.Contracts/`
