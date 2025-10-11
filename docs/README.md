# CrossMilo Contracts - Application Service Contracts

CrossMilo contains the application-level service contracts that define interfaces for various features and capabilities in the WingedBean ecosystem.

## What Was Moved

17 contract projects were moved from `yokan-projects/winged-bean/development/dotnet/framework` to `plate-projects/cross-milo/dotnet/framework`:

### Contracts Moved (with Namespace Change: WingedBean.Contracts.* → Plate.CrossMilo.Contracts.*)

1. **Plate.CrossMilo.Contracts.Analytics** - Analytics and tracking services
2. **Plate.CrossMilo.Contracts.Audio** - Audio playback and management
3. **Plate.CrossMilo.Contracts.Config** - Configuration services
4. **Plate.CrossMilo.Contracts.Diagnostics** - Diagnostics and monitoring
5. **Plate.CrossMilo.Contracts.ECS** - Entity Component System contracts
6. **Plate.CrossMilo.Contracts.FigmaSharp** - Figma integration contracts
7. **Plate.CrossMilo.Contracts.Game** - Game-specific services
8. **Plate.CrossMilo.Contracts.Hosting** - Hosting abstractions
9. **Plate.CrossMilo.Contracts.Input** - Input handling contracts
10. **Plate.CrossMilo.Contracts.Recorder** - Recording services
11. **Plate.CrossMilo.Contracts.Resilience** - Resilience and fault tolerance
12. **Plate.CrossMilo.Contracts.Resource** - Resource management
13. **Plate.CrossMilo.Contracts.Scene** - Scene management
14. **Plate.CrossMilo.Contracts.Terminal** - Terminal abstractions
15. **Plate.CrossMilo.Contracts.TerminalUI** - Terminal UI services
16. **Plate.CrossMilo.Contracts.UI** - UI abstractions
17. **Plate.CrossMilo.Contracts.WebSocket** - WebSocket communication

### NOT Moved

- **WingedBean.Contracts.Core** - Moved to `plugin-manoi` as `PluginManoi.Contracts` (plugin infrastructure)

## Dependencies

### External Dependencies

Cross-milo contracts depend on:

**plugin-manoi** (for plugin infrastructure):
- `PluginManoi.Contracts` - Provides `IRegistry`, `IPlugin`, `SelectionMode`, etc.
  - Path: `../../../../plugin-manoi/dotnet/framework/src/PluginManoi.Contracts/`

**winged-bean** (for code generation):
- `WingedBean.SourceGenerators.Proxy` - Source generator for proxy services
  - Path: `../../../../../yokan-projects/winged-bean/development/dotnet/framework/src/WingedBean.SourceGenerators.Proxy/`

### Package Dependencies

- PolySharp (for C# language feature polyfills)
- System.Reactive (for reactive extensions)
- Various Microsoft.Extensions.* packages

## Build Status

✅ **Successfully building:**
- CrossMilo.Contracts.Analytics
- CrossMilo.Contracts.ECS
- (Simple contracts without proxy services)

⚠️ **Require fixes:**
- Contracts with ProxyService implementations need source generator references resolved
- Some contracts may have additional cross-dependencies to resolve

## Architecture

The contracts in cross-milo define service interfaces that can have multiple implementations:

```csharp
// Example: Audio contract
namespace Plate.CrossMilo.Contracts.Audio;

public interface IAudioService
{
    void Play(string clipId, AudioPlayOptions? options = null);
    void Stop(string clipId);
}
```

These contracts are consumed by:
1. **Platform implementations** (in winged-bean) - actual service implementations
2. **Proxy services** (generated) - registry-based service resolution
3. **Application code** - consumers of the services

## Next Steps

1. Resolve remaining build issues for contracts with proxy services
2. Update winged-bean to reference cross-milo contracts instead of local copies
3. Test end-to-end integration with winged-bean applications
4. Consider organizing contracts into logical solution folders

## Related Projects

- **plugin-manoi** (`../../../../plugin-manoi/`) - Plugin management infrastructure
- **winged-bean** (`../../../../../yokan-projects/winged-bean/`) - Main application framework

## Migration Notes

All namespaces were updated from `WingedBean.Contracts.*` to `Plate.CrossMilo.Contracts.*`. 

Projects that depend on plugin system types now reference `PluginManoi.Contracts` for:
- `IRegistry` - Service registry
- `IPlugin` - Plugin interface
- `SelectionMode` - Service selection strategy
- Attributes for service registration and selection

This creates a clean architectural separation:
- **plugin-manoi**: Low-level plugin infrastructure
- **cross-milo**: Application-level service contracts
- **winged-bean**: Application implementations and hosting
