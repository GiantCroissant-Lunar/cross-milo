# CrossMilo Contracts Reorganization Summary

## New Organizational Pattern

All contract projects now follow a consistent pattern with services organized in subdirectories.

### Pattern Structure

```
CrossMilo.Contracts.{Name}/
├── {SupportingTypes}.cs        # Root namespace: Plate.CrossMilo.Contracts.{Name}
└── Services/
    ├── Interfaces/
    │   └── IService.cs         # Namespace: Plate.CrossMilo.Contracts.{Name}.Services
    └── Service.cs              # ProxyService implementation
```

### Contracts Reorganized

#### 1. CrossMilo.Contracts.Audio
```
CrossMilo.Contracts.Audio/
├── AudioPlayOptions.cs         # Plate.CrossMilo.Contracts.Audio
└── Services/
    ├── Interfaces/
    │   └── IService.cs         # IAudioService → IService
    └── Service.cs              # ProxyService
```

#### 2. CrossMilo.Contracts.Config
```
CrossMilo.Contracts.Config/
├── ConfigChangedEventArgs.cs
├── IConfigSection.cs
└── Services/
    ├── Interfaces/
    │   └── IService.cs         # IConfigService → IService
    └── Service.cs
```

#### 3. CrossMilo.Contracts.Resilience
```
CrossMilo.Contracts.Resilience/
├── ResilienceOptions.cs
├── ResiliencePolicy.cs
└── Services/
    ├── Interfaces/
    │   └── IService.cs         # IResilienceService → IService
    └── Service.cs
```

#### 4. CrossMilo.Contracts.Resource
```
CrossMilo.Contracts.Resource/
├── ResourceMetadata.cs
└── Services/
    ├── Interfaces/
    │   └── IService.cs         # IResourceService → IService
    └── Service.cs
```

#### 5. CrossMilo.Contracts.TerminalUI
```
CrossMilo.Contracts.TerminalUI/
├── ITerminalApp.cs
└── Services/
    ├── Interfaces/
    │   └── IService.cs         # ITerminalUIService → IService
    └── Service.cs
```

#### 6. CrossMilo.Contracts.WebSocket
```
CrossMilo.Contracts.WebSocket/
├── WebSocketMessage.cs
└── Services/
    ├── Interfaces/
    │   └── IService.cs         # IWebSocketService → IService
    └── Service.cs
```

#### 7. CrossMilo.Contracts.Game (Multiple Services)
```
CrossMilo.Contracts.Game/
├── GameEvents.cs
├── GameInputEvent.cs
├── GameTypes.cs
├── RenderBuffer.cs
├── GameUI/
│   ├── Interfaces/
│   │   └── IService.cs         # IGameUIService → IService
│   └── Service.cs              # Namespace: Plate.CrossMilo.Contracts.Game.GameUI
├── Render/
│   ├── Interfaces/
│   │   └── IService.cs         # IRenderService → IService
│   └── Service.cs              # Namespace: Plate.CrossMilo.Contracts.Game.Render
└── Dungeon/
    └── Interfaces/
        └── IService.cs         # IDungeonGameService → IService
```

## Benefits of This Pattern

1. **Consistent Structure**: All contracts follow the same organizational pattern
2. **Clear Separation**: Supporting types at root, services in subdirectories
3. **Simplified Naming**: Generic `IService` name within each service's namespace
4. **Better Scalability**: Easy to add multiple services per contract (see Game example)
5. **Namespace Clarity**: Full namespace provides context (e.g., `Plate.CrossMilo.Contracts.Audio.Services.IService`)

## Namespace Structure

### Root Namespace (Supporting Types)
```csharp
namespace Plate.CrossMilo.Contracts.{ContractName};

// Example: AudioPlayOptions, GameEvents, etc.
```

### Service Namespace (Single Service)
```csharp
namespace Plate.CrossMilo.Contracts.{ContractName}.Services;

public interface IService { ... }
public partial class ProxyService : IService { ... }
```

### Service Namespace (Multiple Services)
```csharp
namespace Plate.CrossMilo.Contracts.Game.{ServiceName};

// Example: Plate.CrossMilo.Contracts.Game.GameUI
// Example: Plate.CrossMilo.Contracts.Game.Render
```

## Source Generator References

All contracts with proxy services now reference:
```xml
<ProjectReference Include="../CrossMilo.SourceGenerators.Proxy/CrossMilo.SourceGenerators.Proxy.csproj" 
                  OutputItemType="Analyzer" 
                  ReferenceOutputAssembly="false" />
```

#### 8. CrossMilo.Contracts.Analytics
```
CrossMilo.Contracts.Analytics/
├── AnalyticsModels.cs
└── Services/
    └── Interfaces/
        └── IService.cs         # IAnalyticsService → IService
```

#### 9. CrossMilo.Contracts.Diagnostics
```
CrossMilo.Contracts.Diagnostics/
├── DiagnosticsModels.cs
└── Services/
    └── Interfaces/
        └── IService.cs         # IDiagnosticsService → IService
```

#### 10. CrossMilo.Contracts.ECS
```
CrossMilo.Contracts.ECS/
├── IEntity.cs, IWorld.cs, IQuery.cs, IECSSystem.cs  # Supporting interfaces
├── EntityHandle.cs, WorldHandle.cs, etc.
└── Services/
    └── Interfaces/
        └── IService.cs         # IECSService → IService
```

#### 11. CrossMilo.Contracts.FigmaSharp (Multiple Services)
```
CrossMilo.Contracts.FigmaSharp/
├── Color.cs, FigmaEnums.cs, FObject.cs, etc.
├── Transformer/
│   └── Interfaces/
│       └── IService.cs         # IFigmaTransformer → IService
└── Renderer/
    └── Interfaces/
        └── IService.cs         # IUIRenderer → IService
```

#### 12. CrossMilo.Contracts.Hosting (Multiple Services)
```
CrossMilo.Contracts.Hosting/
├── App/
│   └── Interfaces/
│       └── IService.cs         # IWingedBeanApp → IService
└── Host/
    └── Interfaces/
        └── IService.cs         # IWingedBeanHost → IService
```

#### 13. CrossMilo.Contracts.Input (Multiple Services)
```
CrossMilo.Contracts.Input/
├── RawKeyEvent.cs
├── Scope/
│   └── Interfaces/
│       └── IService.cs         # IInputScope → IService
├── Router/
│   └── Interfaces/
│       └── IService.cs         # IInputRouter → IService
└── Mapper/
    └── Interfaces/
        └── IService.cs         # IInputMapper → IService
```

#### 14. CrossMilo.Contracts.Recorder
```
CrossMilo.Contracts.Recorder/
└── Services/
    └── Interfaces/
        └── IService.cs         # IRecorder → IService
```

#### 15. CrossMilo.Contracts.Scene
```
CrossMilo.Contracts.Scene/
├── Camera.cs, SceneLayer.cs, Viewport.cs, etc.
└── Services/
    └── Interfaces/
        └── IService.cs         # ISceneService → IService
```

#### 16. CrossMilo.Contracts.Terminal
```
CrossMilo.Contracts.Terminal/
├── LegacyTerminalAppAdapter.cs
└── App/
    └── Interfaces/
        └── IService.cs         # ITerminalApp → IService
```

#### 17. CrossMilo.Contracts.UI
```
CrossMilo.Contracts.UI/
└── App/
    └── Interfaces/
        └── IService.cs         # IUIApp → IService
```

## Status

✅ **ALL 17 CONTRACTS REORGANIZED**

All contracts now follow the consistent organizational pattern:
- **With Proxy Services (7)**: Audio, Config, Game, Resilience, Resource, TerminalUI, WebSocket
- **Without Proxy Services Yet (10)**: Analytics, Diagnostics, ECS, FigmaSharp, Hosting, Input, Recorder, Scene, Terminal, UI

All contracts are now structured consistently and ready for use or future proxy service addition.
