# ⚠️ DEPRECATED

**This project is deprecated and should not be used in new code.**

## Migration Date
2025-10-11

## Reason
ECS (Entity Component System) abstractions have been moved to be internal implementation details within the ArchECS plugin. Game-specific logic should not depend on framework-level ECS contracts.

## Migration Path

### Before (Old)
```csharp
using Plate.CrossMilo.Contracts.ECS.Services;

// Using framework ECS contract
public class MyGame
{
    private readonly IService _ecsService;
    
    public MyGame(IRegistry registry)
    {
        _ecsService = registry.Get<IService>();
    }
}
```

### After (New)
```csharp
using WingedBean.Plugins.ArchECS;
using WingedBean.Plugins.ArchECS.Internal;

// Using ArchECS directly
public class MyGame
{
    private readonly ArchECSService _ecsService;
    
    public MyGame(ArchECSService ecsService)
    {
        _ecsService = ecsService;
    }
}
```

## Replacement

- **Old**: `Plate.CrossMilo.Contracts.ECS`
- **New**: `WingedBean.Plugins.ArchECS.Internal` (for internal ECS types)
- **New**: `ArchECSService` (for service registration)

## Status

- ❌ **Removed from build-config.json** - Will not be built in CI/CD
- ⚠️ **Still in solution** - For backward compatibility only
- 🔒 **No new features** - This project is frozen

## See Also

- Migration document: `/Users/apprenticegc/Work/lunar-horse/personal-work/yokan-projects/winged-bean/development/dotnet/console/MIGRATION_GAME_CONTRACTS.md`
- ArchECS plugin: `/Users/apprenticegc/Work/lunar-horse/personal-work/yokan-projects/winged-bean/development/dotnet/console/src/plugins/WingedBean.Plugins.ArchECS/`
