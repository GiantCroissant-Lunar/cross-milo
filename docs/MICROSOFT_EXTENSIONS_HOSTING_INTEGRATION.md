# Microsoft.Extensions.Hosting Integration

**Date:** 2025-10-11  
**Status:** Active Design Document

## Overview

`CrossMilo.Contracts.Hosting` **wraps and extends** Microsoft.Extensions.Hosting, not replaces it. This ensures compatibility with the .NET ecosystem while adding platform-agnostic application lifecycle management.

**Naming Decision:** The name `Hosting` is intentionally kept to maintain consistency with `Microsoft.Extensions.Hosting`. In the .NET ecosystem, "Hosting" means "application lifecycle management", not "web hosting" or "graphics backend".

## Design Principle

**CrossMilo.Contracts.Hosting = Microsoft.Extensions.Hosting + CrossMilo Extensions**

```
┌─────────────────────────────────────────────────────────────┐
│ CrossMilo.Contracts.Hosting                                 │
│ - Platform-agnostic application contracts                   │
│ - Runtime/Edit-time mode support                            │
│ - Extended lifecycle events                                 │
└─────────────────────────────────────────────────────────────┘
                         ↓ extends
┌─────────────────────────────────────────────────────────────┐
│ Microsoft.Extensions.Hosting                                │
│ - IHostedService (standard .NET lifecycle)                  │
│ - IHost, IHostBuilder                                       │
│ - Dependency Injection (IServiceProvider)                   │
│ - Configuration (IConfiguration)                            │
│ - Logging (ILogger)                                         │
└─────────────────────────────────────────────────────────────┘
```

## Current Implementation

### CrossMilo.Contracts.Hosting.App.IService

Already extends `IHostedService`:

```csharp
using Microsoft.Extensions.Hosting;

namespace Plate.CrossMilo.Contracts.Hosting.App;

/// <summary>
/// Base interface for all applications.
/// Extends IHostedService for compatibility with Microsoft.Extensions.Hosting.
/// </summary>
public interface IService : IHostedService
{
    /// <summary>Application name (for logging, diagnostics)</summary>
    string Name { get; }

    /// <summary>Current application state</summary>
    AppState State { get; }

    /// <summary>Fired when application state changes</summary>
    event EventHandler<AppStateChangedEventArgs>? StateChanged;
}

/// <summary>Application lifecycle states</summary>
public enum AppState
{
    NotStarted,
    Starting,
    Running,
    Stopping,
    Stopped,
    Faulted
}
```

**Key Points:**
- ✅ **Extends `IHostedService`** - Compatible with .NET Generic Host
- ✅ **Adds application state tracking** - Beyond basic start/stop
- ✅ **Provides state change events** - For monitoring and debugging

## Proposed Extension: Runtime/Edit-Time Mode Support

### Add Mode Management to IService

```csharp
using Microsoft.Extensions.Hosting;

namespace Plate.CrossMilo.Contracts.Hosting.App;

/// <summary>
/// Base interface for all applications.
/// Extends IHostedService for compatibility with Microsoft.Extensions.Hosting.
/// Adds support for runtime/edit-time mode switching.
/// </summary>
public interface IService : IHostedService
{
    /// <summary>Application name (for logging, diagnostics)</summary>
    string Name { get; }

    /// <summary>Current application state</summary>
    AppState State { get; }

    /// <summary>Current application mode (runtime or edit-time)</summary>
    AppMode CurrentMode { get; }

    /// <summary>Switch between runtime and edit-time modes</summary>
    /// <param name="mode">Target mode</param>
    /// <param name="ct">Cancellation token</param>
    Task SwitchModeAsync(AppMode mode, CancellationToken ct = default);

    /// <summary>Fired when application state changes</summary>
    event EventHandler<AppStateChangedEventArgs>? StateChanged;

    /// <summary>Fired when application mode changes</summary>
    event EventHandler<AppModeChangedEventArgs>? ModeChanged;
}

/// <summary>Application mode enumeration</summary>
public enum AppMode
{
    /// <summary>Runtime mode - application is running for end-users</summary>
    Runtime,

    /// <summary>Edit-time mode - application is being authored/configured</summary>
    EditTime
}

public class AppModeChangedEventArgs : EventArgs
{
    public AppMode PreviousMode { get; set; }
    public AppMode NewMode { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
```

## Integration with Microsoft.Extensions.Hosting

### Console Host Example (WingedBean)

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Plate.CrossMilo.Contracts.Hosting.App;

namespace WingedBean.Host;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Use Microsoft.Extensions.Hosting.Host
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Register CrossMilo application service
                services.AddSingleton<IService, WingedBeanApp>();
                
                // Register plugins
                services.AddSingleton<IPluginLoader, PluginLoader>();
                
                // Register other services
                services.AddLogging();
                services.AddOptions();
            })
            .Build();

        // Run the host (starts all IHostedService implementations)
        await host.RunAsync();
    }
}

/// <summary>
/// WingedBean console application.
/// Implements CrossMilo.Contracts.Hosting.App.IService
/// which extends Microsoft.Extensions.Hosting.IHostedService.
/// </summary>
public class WingedBeanApp : IService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WingedBeanApp> _logger;
    private AppState _state = AppState.NotStarted;
    private AppMode _currentMode = AppMode.Runtime;

    public string Name => "WingedBean Console";
    public AppState State => _state;
    public AppMode CurrentMode => _currentMode;

    public event EventHandler<AppStateChangedEventArgs>? StateChanged;
    public event EventHandler<AppModeChangedEventArgs>? ModeChanged;

    public WingedBeanApp(
        IServiceProvider serviceProvider,
        ILogger<WingedBeanApp> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    // IHostedService.StartAsync (required by Microsoft.Extensions.Hosting)
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {AppName} in {Mode} mode", Name, CurrentMode);
        
        ChangeState(AppState.Starting);
        
        try
        {
            // Load plugins
            var pluginLoader = _serviceProvider.GetRequiredService<IPluginLoader>();
            await pluginLoader.LoadPluginsAsync(cancellationToken);
            
            // Start in runtime mode by default
            await SwitchModeAsync(AppMode.Runtime, cancellationToken);
            
            ChangeState(AppState.Running);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start {AppName}", Name);
            ChangeState(AppState.Faulted);
            throw;
        }
    }

    // IHostedService.StopAsync (required by Microsoft.Extensions.Hosting)
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping {AppName}", Name);
        
        ChangeState(AppState.Stopping);
        
        try
        {
            // Cleanup resources
            await Task.CompletedTask;
            
            ChangeState(AppState.Stopped);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during shutdown of {AppName}", Name);
            ChangeState(AppState.Faulted);
            throw;
        }
    }

    // CrossMilo extension: Mode switching
    public async Task SwitchModeAsync(AppMode mode, CancellationToken ct = default)
    {
        if (_currentMode == mode)
            return;

        _logger.LogInformation("Switching from {OldMode} to {NewMode}", _currentMode, mode);

        var previousMode = _currentMode;
        _currentMode = mode;

        // Notify mode change
        ModeChanged?.Invoke(this, new AppModeChangedEventArgs
        {
            PreviousMode = previousMode,
            NewMode = mode,
            Timestamp = DateTimeOffset.UtcNow
        });

        // Load appropriate UI for the mode
        if (mode == AppMode.Runtime)
        {
            await LoadRuntimeUIAsync(ct);
        }
        else if (mode == AppMode.EditTime)
        {
            await LoadEditorUIAsync(ct);
        }
    }

    private void ChangeState(AppState newState)
    {
        var previousState = _state;
        _state = newState;

        StateChanged?.Invoke(this, new AppStateChangedEventArgs
        {
            PreviousState = previousState,
            NewState = newState
        });
    }

    private async Task LoadRuntimeUIAsync(CancellationToken ct)
    {
        _logger.LogInformation("Loading runtime UI");
        // Load Terminal.Gui runtime UI
        await Task.CompletedTask;
    }

    private async Task LoadEditorUIAsync(CancellationToken ct)
    {
        _logger.LogInformation("Loading editor UI");
        // Load Terminal.Gui editor UI (different layout)
        await Task.CompletedTask;
    }
}
```

## Unity Host Integration

Unity doesn't use `Microsoft.Extensions.Hosting` by default, but we can still integrate:

### Option 1: Use Microsoft.Extensions.Hosting in Unity

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using Plate.CrossMilo.Contracts.Application.App;

namespace UnityHost;

/// <summary>
/// Unity MonoBehaviour that hosts Microsoft.Extensions.Hosting.
/// Bridges Unity lifecycle with .NET Generic Host.
/// </summary>
public class UnityHostBootstrapper : MonoBehaviour
{
    private IHost? _host;

    private async void Start()
    {
        // Create .NET Generic Host in Unity
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register CrossMilo application
                services.AddSingleton<IService, UnityApp>();
                
                // Register Unity-specific services
                services.AddSingleton<IUnityContext>(new UnityContext(this));
                
                // Register plugins
                services.AddSingleton<IPluginLoader, UnityPluginLoader>();
            })
            .Build();

        // Start the host
        await _host.StartAsync();
    }

    private async void OnDestroy()
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}

/// <summary>
/// Unity application implementation.
/// Implements CrossMilo.Contracts.Hosting.App.IService.
/// </summary>
public class UnityApp : IService
{
    private readonly IUnityContext _unityContext;
    private readonly ILogger<UnityApp> _logger;
    private AppState _state = AppState.NotStarted;
    private AppMode _currentMode = AppMode.Runtime;

    public string Name => "Unity Host";
    public AppState State => _state;
    public AppMode CurrentMode => _currentMode;

    public event EventHandler<AppStateChangedEventArgs>? StateChanged;
    public event EventHandler<AppModeChangedEventArgs>? ModeChanged;

    public UnityApp(IUnityContext unityContext, ILogger<UnityApp> logger)
    {
        _unityContext = unityContext;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Unity app");
        
        // Detect if we're in Unity Editor or Play mode
        if (Application.isEditor)
        {
            _currentMode = AppMode.EditTime;
        }
        else
        {
            _currentMode = AppMode.Runtime;
        }
        
        ChangeState(AppState.Running);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Unity app");
        ChangeState(AppState.Stopped);
        await Task.CompletedTask;
    }

    public async Task SwitchModeAsync(AppMode mode, CancellationToken ct = default)
    {
        // In Unity, mode switching is controlled by Unity Editor
        // (Edit Mode vs Play Mode)
        _logger.LogWarning("Mode switching in Unity is controlled by Unity Editor");
        await Task.CompletedTask;
    }

    private void ChangeState(AppState newState)
    {
        var previousState = _state;
        _state = newState;
        StateChanged?.Invoke(this, new AppStateChangedEventArgs
        {
            PreviousState = previousState,
            NewState = newState
        });
    }
}
```

### Option 2: Adapt CrossMilo to Unity Lifecycle (No Microsoft.Extensions.Hosting)

```csharp
using UnityEngine;
using Plate.CrossMilo.Contracts.Hosting.App;

namespace UnityHost;

/// <summary>
/// Unity-specific application implementation.
/// Adapts CrossMilo.Contracts.Hosting to Unity lifecycle.
/// Does NOT use Microsoft.Extensions.Hosting (Unity doesn't need it).
/// </summary>
public class UnityApp : MonoBehaviour, IService
{
    private AppState _state = AppState.NotStarted;
    private AppMode _currentMode = AppMode.Runtime;

    public string Name => "Unity Host";
    public AppState State => _state;
    public AppMode CurrentMode => _currentMode;

    public event EventHandler<AppStateChangedEventArgs>? StateChanged;
    public event EventHandler<AppModeChangedEventArgs>? ModeChanged;

    // Unity lifecycle methods
    private void Awake()
    {
        // Detect mode
        if (Application.isEditor)
        {
            _currentMode = AppMode.EditTime;
        }
    }

    private async void Start()
    {
        await StartAsync(default);
    }

    private async void OnDestroy()
    {
        await StopAsync(default);
    }

    // IHostedService implementation (for CrossMilo compatibility)
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Debug.Log($"Starting {Name} in {CurrentMode} mode");
        ChangeState(AppState.Running);
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Debug.Log($"Stopping {Name}");
        ChangeState(AppState.Stopped);
        await Task.CompletedTask;
    }

    public async Task SwitchModeAsync(AppMode mode, CancellationToken ct = default)
    {
        Debug.LogWarning("Mode switching in Unity is controlled by Unity Editor");
        await Task.CompletedTask;
    }

    private void ChangeState(AppState newState)
    {
        var previousState = _state;
        _state = newState;
        StateChanged?.Invoke(this, new AppStateChangedEventArgs
        {
            PreviousState = previousState,
            NewState = newState
        });
    }
}
```

## Godot Host Integration

Similar to Unity, Godot can use Microsoft.Extensions.Hosting or adapt to Godot lifecycle:

```csharp
using Godot;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Plate.CrossMilo.Contracts.Hosting.App;

namespace GodotHost;

/// <summary>
/// Godot Node that hosts Microsoft.Extensions.Hosting.
/// Bridges Godot lifecycle with .NET Generic Host.
/// </summary>
public partial class GodotHostBootstrapper : Node
{
    private IHost? _host;

    public override async void _Ready()
    {
        // Create .NET Generic Host in Godot
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Register CrossMilo application
                services.AddSingleton<IService, GodotApp>();
                
                // Register Godot-specific services
                services.AddSingleton<IGodotContext>(new GodotContext(this));
                
                // Register plugins
                services.AddSingleton<IPluginLoader, GodotPluginLoader>();
            })
            .Build();

        // Start the host
        await _host.StartAsync();
    }

    public override async void _ExitTree()
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}
```

## Benefits of Microsoft.Extensions.Hosting Integration

### 1. **Standard .NET Ecosystem Compatibility**

✅ Works with existing .NET tools and libraries  
✅ Familiar to .NET developers  
✅ Well-documented and battle-tested  

### 2. **Built-in Features**

✅ **Dependency Injection** - `IServiceProvider`, `IServiceCollection`  
✅ **Configuration** - `IConfiguration`, appsettings.json  
✅ **Logging** - `ILogger`, structured logging  
✅ **Options Pattern** - `IOptions<T>`, strongly-typed configuration  
✅ **Hosted Services** - `IHostedService`, background tasks  

### 3. **Extensibility**

✅ Can add custom `IHostedService` implementations  
✅ Can use third-party libraries (Serilog, NLog, etc.)  
✅ Can integrate with ASP.NET Core (for web hosting)  

### 4. **CrossMilo Extensions**

✅ **Mode switching** - Runtime ↔ Edit-time  
✅ **State tracking** - Beyond basic start/stop  
✅ **Platform abstraction** - Works in Console, Unity, Godot  

## Customization Points

### 1. Custom IHostBuilder Extensions

```csharp
public static class CrossMiloHostBuilderExtensions
{
    /// <summary>
    /// Add CrossMilo application services to the host.
    /// </summary>
    public static IHostBuilder UseCrossMilo(
        this IHostBuilder builder,
        Action<CrossMiloOptions>? configure = null)
    {
        return builder.ConfigureServices((context, services) =>
        {
            // Configure CrossMilo options
            var options = new CrossMiloOptions();
            configure?.Invoke(options);
            services.AddSingleton(options);

            // Register CrossMilo application
            services.AddSingleton<IService, CrossMiloApp>();

            // Register plugin loader
            services.AddSingleton<IPluginLoader, PluginLoader>();

            // Register service registry
            services.AddSingleton<IRegistry, ActualRegistry>();
        });
    }
}

// Usage:
var host = Host.CreateDefaultBuilder(args)
    .UseCrossMilo(options =>
    {
        options.PluginDirectory = "./plugins";
        options.DefaultMode = AppMode.Runtime;
    })
    .Build();
```

### 2. Custom Configuration

```csharp
// appsettings.json
{
  "CrossMilo": {
    "Application": {
      "Name": "WingedBean",
      "DefaultMode": "Runtime",
      "EnableModeSwitch": true
    },
    "Plugins": {
      "Directory": "./plugins",
      "AutoLoad": true,
      "Whitelist": ["WingedBean.Plugins.*"]
    }
  }
}

// Configuration binding
public class CrossMiloOptions
{
    public ApplicationOptions Application { get; set; } = new();
    public PluginOptions Plugins { get; set; } = new();
}

public class ApplicationOptions
{
    public string Name { get; set; } = "CrossMilo App";
    public AppMode DefaultMode { get; set; } = AppMode.Runtime;
    public bool EnableModeSwitch { get; set; } = true;
}

public class PluginOptions
{
    public string Directory { get; set; } = "./plugins";
    public bool AutoLoad { get; set; } = true;
    public List<string> Whitelist { get; set; } = new();
}
```

### 3. Custom Logging

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((context, logging) =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();
        
        // Add Serilog for structured logging
        logging.AddSerilog(new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/crossmilo-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger());
    })
    .UseCrossMilo()
    .Build();
```

## Summary

### Key Takeaways

1. **CrossMilo.Contracts.Hosting extends Microsoft.Extensions.Hosting**
   - Not a replacement, but an extension
   - `IService` extends `IHostedService`
   - Fully compatible with .NET Generic Host
   - Name kept as "Hosting" for .NET ecosystem consistency

2. **Console Host uses Microsoft.Extensions.Hosting directly**
   - Standard .NET application pattern
   - Full access to DI, configuration, logging
   - Can use all .NET ecosystem libraries

3. **Unity/Godot can optionally use Microsoft.Extensions.Hosting**
   - Option 1: Host Microsoft.Extensions.Hosting in Unity/Godot
   - Option 2: Adapt CrossMilo contracts to Unity/Godot lifecycle
   - Both approaches work, choose based on needs

4. **CrossMilo adds platform-agnostic extensions**
   - Mode switching (Runtime ↔ Edit-time)
   - State tracking (NotStarted, Starting, Running, etc.)
   - Platform abstraction (works everywhere)

5. **Customization through standard .NET patterns**
   - `IHostBuilder` extensions
   - Configuration binding (`IOptions<T>`)
   - Custom logging providers
   - Third-party library integration

### Advantages

✅ **Standard .NET patterns** - Familiar to .NET developers  
✅ **Ecosystem compatibility** - Works with existing libraries  
✅ **Battle-tested** - Microsoft.Extensions.Hosting is production-ready  
✅ **Extensible** - Easy to customize and extend  
✅ **Platform-agnostic** - CrossMilo contracts work everywhere  

## References

- **Microsoft.Extensions.Hosting:** https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host
- **IHostedService:** https://docs.microsoft.com/en-us/dotnet/core/extensions/hosted-services
- **Dependency Injection:** https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection
- **Configuration:** https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration
- **Logging:** https://docs.microsoft.com/en-us/dotnet/core/extensions/logging
