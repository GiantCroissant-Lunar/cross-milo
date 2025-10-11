# Naming Decision: CrossMilo.Contracts.Hosting

**Date:** 2025-10-11  
**Status:** Final Decision  
**Decision:** Keep the name `CrossMilo.Contracts.Hosting` (do NOT rename to `Application`)

## Summary

The name **`CrossMilo.Contracts.Hosting`** is intentionally kept to maintain consistency with `Microsoft.Extensions.Hosting`. This decision reflects the fact that CrossMilo directly extends and integrates with the .NET Generic Host pattern.

## Rationale

### 1. Direct Integration with Microsoft.Extensions.Hosting

`CrossMilo.Contracts.Hosting.App.IService` **extends** `IHostedService` from `Microsoft.Extensions.Hosting.Abstractions`:

```csharp
using Microsoft.Extensions.Hosting;

namespace Plate.CrossMilo.Contracts.Hosting.App;

/// <summary>
/// Base interface for all applications.
/// Extends IHostedService for compatibility with Microsoft.Extensions.Hosting.
/// </summary>
public interface IService : IHostedService
{
    string Name { get; }
    AppState State { get; }
    event EventHandler<AppStateChangedEventArgs>? StateChanged;
}
```

**Key Point:** Since we extend `IHostedService`, the name `Hosting` is the natural choice.

### 2. .NET Ecosystem Naming Consistency

In the .NET ecosystem, **"Hosting" means "application lifecycle management"**, not "web hosting" or "graphics backend":

- `Microsoft.Extensions.Hosting` - Generic host for .NET applications
- `Microsoft.Extensions.Hosting.Abstractions` - Core hosting abstractions
- `IHostedService` - Service that participates in application lifecycle
- `IHost` - Represents a running application
- `IHostBuilder` - Builds and configures the host

**By using `Hosting`, developers immediately understand the relationship.**

### 3. Avoids Confusion with "Application" Term

The term "Application" is overloaded in software:
- Application layer (in layered architecture)
- Application services (in DDD)
- Application logic (vs infrastructure logic)
- Application UI (vs system UI)

**"Hosting" is more specific and unambiguous in the .NET context.**

### 4. Comparison with SadConsole

The initial concern was that "Hosting" might be confused with SadConsole's "Host" concept:

**SadConsole's "Host":**
- Means graphics/rendering backend (FNA, SFML, MonoGame)
- Not related to application lifecycle
- Platform-specific rendering implementation

**CrossMilo's "Hosting":**
- Means application lifecycle management
- Extends Microsoft.Extensions.Hosting
- Platform-agnostic application contracts

**These are different concepts in different contexts.** Since CrossMilo is a .NET-first framework that directly integrates with Microsoft.Extensions.Hosting, the .NET naming convention takes precedence.

## What "Hosting" Means in CrossMilo

### In .NET Context (Console Host)

```csharp
// Standard .NET Generic Host pattern
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register CrossMilo application (extends IHostedService)
        services.AddSingleton<IService, WingedBeanApp>();
    })
    .Build();

await host.RunAsync();
```

**"Hosting" = Application lifecycle management using Microsoft.Extensions.Hosting**

### In Unity/Godot Context

```csharp
// Unity/Godot can optionally use Microsoft.Extensions.Hosting
var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IService, UnityApp>();
    })
    .Build();
```

**"Hosting" = Same concept, optionally using Microsoft.Extensions.Hosting**

## Package Structure

```
CrossMilo.Contracts.Hosting/
├── App/
│   └── Interfaces/
│       └── IService.cs           ← Extends IHostedService
├── Host/
│   └── Interfaces/
│       └── IService.cs           ← Host-specific contracts
└── CrossMilo.Contracts.Hosting.csproj
    └── References:
        - Microsoft.Extensions.Hosting.Abstractions
        - Microsoft.Extensions.DependencyInjection.Abstractions
        - Microsoft.Extensions.Configuration.Abstractions
        - Microsoft.Extensions.Logging.Abstractions
```

**Package references clearly show the relationship with Microsoft.Extensions.Hosting.**

## Benefits of Keeping "Hosting"

### ✅ 1. Immediate Recognition

.NET developers see `CrossMilo.Contracts.Hosting` and immediately understand:
- It's related to application lifecycle
- It likely extends Microsoft.Extensions.Hosting
- It follows .NET patterns and conventions

### ✅ 2. Ecosystem Compatibility

- Works seamlessly with .NET Generic Host
- Compatible with ASP.NET Core hosting
- Integrates with existing .NET tools and libraries
- Familiar to .NET developers

### ✅ 3. Clear Relationship

The name makes the relationship explicit:
```
Microsoft.Extensions.Hosting
         ↓ extends
CrossMilo.Contracts.Hosting
         ↓ implements
WingedBean.Host (Console)
UnityHost (Unity)
GodotHost (Godot)
```

### ✅ 4. Consistent Terminology

Using "Hosting" throughout the stack:
- `Microsoft.Extensions.Hosting` - .NET framework
- `CrossMilo.Contracts.Hosting` - CrossMilo contracts
- `IHostedService` - Service interface
- `IHost` - Application host
- `IHostBuilder` - Host builder

## Alternative Names Considered

### ❌ CrossMilo.Contracts.Application

**Rejected because:**
- "Application" is too generic and overloaded
- Doesn't convey the relationship with Microsoft.Extensions.Hosting
- Less clear to .NET developers
- Breaks naming consistency with .NET ecosystem

### ❌ CrossMilo.Contracts.Lifecycle

**Rejected because:**
- Not a standard .NET term
- Doesn't indicate Microsoft.Extensions.Hosting integration
- Less familiar to .NET developers

### ❌ CrossMilo.Contracts.Runtime

**Rejected because:**
- "Runtime" implies execution environment, not lifecycle management
- Confusing with runtime mode vs edit-time mode
- Not aligned with .NET terminology

## Addressing the SadConsole Comparison

**Question:** Won't developers confuse CrossMilo's "Hosting" with SadConsole's "Host"?

**Answer:** No, because:

1. **Different contexts:**
   - SadConsole is a console game engine (niche)
   - CrossMilo is a .NET-first framework (mainstream)

2. **Different meanings:**
   - SadConsole "Host" = Graphics backend
   - CrossMilo "Hosting" = Application lifecycle (standard .NET term)

3. **Target audience:**
   - .NET developers are familiar with Microsoft.Extensions.Hosting
   - They will recognize the pattern immediately

4. **Documentation clarity:**
   - We explicitly document the difference
   - We explain the relationship with Microsoft.Extensions.Hosting
   - We provide clear examples

## Final Decision

**Keep the name `CrossMilo.Contracts.Hosting`** because:

1. ✅ Extends `Microsoft.Extensions.Hosting` directly
2. ✅ Follows .NET ecosystem naming conventions
3. ✅ Immediately recognizable to .NET developers
4. ✅ Maintains consistency with `IHostedService`, `IHost`, `IHostBuilder`
5. ✅ Avoids overloaded term "Application"
6. ✅ Clear and unambiguous in .NET context

## References

- **Microsoft.Extensions.Hosting:** https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host
- **IHostedService:** https://docs.microsoft.com/en-us/dotnet/core/extensions/hosted-services
- **CrossMilo Architecture:** `ARCHITECTURE_HOST_VS_UI.md`
- **Microsoft.Extensions.Hosting Integration:** `MICROSOFT_EXTENSIONS_HOSTING_INTEGRATION.md`
