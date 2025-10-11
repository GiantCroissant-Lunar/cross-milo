# Architecture: Host vs UI Framework

**Date:** 2025-10-11  
**Status:** Active Design Document

## Overview

This document clarifies the distinction between **Host (Application)**, **UI Framework**, and **Game Logic** in the CrossMilo architecture, and explains how it differs from similar frameworks like SadConsole.

## Key Concepts

### 1. Host (Application Runtime)

**Host** = The application runtime environment where the code executes.

A host provides:
- **Application lifecycle management** (startup, shutdown, services)
- **Dependency injection container**
- **Configuration and logging infrastructure**
- **Plugin loading and activation**
- **Runtime mode AND edit-time mode support** (critical distinction)

### 2. UI Framework (Rendering/Input)

**UI Framework** = The library/system used to render UI and handle user input.

A UI framework provides:
- **Rendering primitives** (windows, buttons, text, etc.)
- **Input handling** (keyboard, mouse, touch)
- **Layout system**
- **Event system**

### 3. Game Logic (CrossMilo Contracts)

**Game Logic** = Platform-agnostic business logic and contracts.

Game logic provides:
- **Domain contracts** (Audio, Scene, Input, ECS, etc.)
- **Service interfaces**
- **Event definitions**
- **Data models**

## Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│ Layer 1: Host (Application Runtime)                        │
│ - Manages application lifecycle                            │
│ - Provides DI, config, logging                             │
│ - Loads plugins                                             │
│ - Supports BOTH runtime mode AND edit-time mode            │
└─────────────────────────────────────────────────────────────┘
                         ↓ uses
┌─────────────────────────────────────────────────────────────┐
│ Layer 2: UI Framework (Rendering/Input)                    │
│ - Renders UI elements                                       │
│ - Handles user input                                        │
│ - Provides layout system                                    │
│ - Can have BOTH runtime UI AND edit-time UI                │
└─────────────────────────────────────────────────────────────┘
                         ↓ implements
┌─────────────────────────────────────────────────────────────┐
│ Layer 3: Game Logic (CrossMilo Contracts)                  │
│ - Platform-agnostic contracts                               │
│ - Business logic interfaces                                 │
│ - Works in ANY host, ANY UI framework                       │
│ - Works in BOTH runtime mode AND edit-time mode             │
└─────────────────────────────────────────────────────────────┘
```

## Runtime Mode vs Edit-Time Mode

### Critical Design Principle

**All hosts support TWO modes:**

1. **Runtime Mode** - Application is running for end-users
2. **Edit-Time Mode** - Application is being authored/configured

This is NOT just a Unity/Godot concept - it applies to ALL hosts!

### Console Host (WingedBean)

```
Console Application (WingedBean)
├── Runtime Mode
│   ├── UI: Terminal.Gui v2 (game UI)
│   ├── UI: ImGui (debug overlay)
│   └── Contracts: CrossMilo.Contracts.* (runtime services)
│
└── Edit-Time Mode
    ├── UI: Terminal.Gui v2 (editor UI - different layout)
    ├── UI: ImGui (property inspector, scene hierarchy)
    └── Contracts: CrossMilo.Contracts.* (same contracts, different implementations)
```

**Console can switch between modes at runtime:**
- Start in runtime mode (playing the game)
- Press hotkey (e.g., F12) to enter edit-time mode
- Edit game state, configure settings, debug
- Press hotkey again to return to runtime mode

### Unity Host

```
Unity Application
├── Edit Mode (Unity Editor)
│   ├── UI: Unity Editor GUI (built-in)
│   ├── UI: Custom Editor Windows (ImGui-style)
│   └── Contracts: CrossMilo.Contracts.* (editor-time implementations)
│
└── Play Mode (Unity Runtime)
    ├── UI: Unity UGUI (game UI)
    ├── UI: Unity UI Toolkit (modern UI)
    └── Contracts: CrossMilo.Contracts.* (runtime implementations)
```

**Unity has distinct modes:**
- Edit mode = Authoring in Unity Editor
- Play mode = Testing/running the game in editor
- Build mode = Standalone game (runtime only)

### Godot Host

```
Godot Application
├── Editor Mode
│   ├── UI: Godot Editor (built-in)
│   ├── UI: Custom EditorPlugins
│   └── Contracts: CrossMilo.Contracts.* (editor-time implementations)
│
└── Runtime Mode
    ├── UI: Godot Control Nodes (game UI)
    └── Contracts: CrossMilo.Contracts.* (runtime implementations)
```

## Comparison: CrossMilo vs SadConsole

| Aspect | SadConsole | CrossMilo |
|--------|-----------|-----------|
| **"Host" means** | Graphics/rendering backend (FNA, SFML, MonoGame) | Application runtime environment (Console, Unity, Godot) |
| **UI Framework** | Always SadConsole (console-style) | Pluggable (Terminal.Gui, ImGui, Blazor, Unity UGUI, etc.) |
| **Game Logic** | Tightly coupled to SadConsole API | Platform-agnostic (CrossMilo.Contracts) |
| **Edit-Time Support** | No built-in editor | Edit-time mode in ALL hosts |
| **Flexibility** | Fixed console-style UI, swappable renderer | Swappable UI framework AND host |
| **Portability** | Console-style games only | Console, Unity, Godot, Web, Desktop |

## Host Implementations

### Console Host (WingedBean)

**Location:** `/yokan-projects/winged-bean`

**Characteristics:**
- .NET console application
- Runs in terminal/PTY
- **Console-first design** (reduce complexity)
- **Portable to Unity/Godot** (same contracts)

**Runtime Mode:**
- Terminal.Gui v2 for game UI
- ImGui for debug overlay (optional)
- Full game experience

**Edit-Time Mode:**
- Terminal.Gui v2 for editor UI (different layout)
- ImGui for property inspector, scene hierarchy
- Edit game state, configure settings
- Hot-reload support

**Mode Switching:**
```csharp
// User presses F12 or menu option
if (Input.IsKeyPressed(Key.F12))
{
    _app.SwitchMode(AppMode.EditTime);
}

// In edit-time mode, user can:
// - Inspect game objects
// - Modify properties
// - Save/load scenes
// - Configure settings
// - Then switch back to runtime mode
```

### Unity Host

**Location:** TBD (future)

**Characteristics:**
- Unity game engine
- Unity Editor for edit-time
- Standalone build for runtime

**Edit Mode (Unity Editor):**
- Unity Editor GUI (built-in)
- Custom Editor Windows (using CrossMilo.Contracts)
- Editor scripting (C# attributes, custom inspectors)

**Play Mode (Unity Runtime):**
- Unity UGUI or UI Toolkit
- CrossMilo contracts implemented via Unity services
- MonoBehaviour-based implementations

**Key Insight:**
- CrossMilo contracts work in BOTH edit mode and play mode
- Different implementations for each mode
- Same game logic, different execution context

### Godot Host

**Location:** TBD (future)

**Characteristics:**
- Godot game engine
- Godot Editor for edit-time
- Standalone build for runtime

**Editor Mode:**
- Godot Editor (built-in)
- EditorPlugins (using CrossMilo.Contracts)
- GDScript or C# editor tools

**Runtime Mode:**
- Godot Control Nodes
- CrossMilo contracts implemented via Godot services
- Node-based implementations

## UI Framework Plugins

### Console Host UI Options

```
WingedBean (Console Host)
├── WingedBean.Plugins.TerminalUI (Terminal.Gui v2)
│   ├── Runtime UI: Game interface
│   └── Edit-Time UI: Editor interface
│
├── WingedBean.Plugins.ImGuiUI (ImGui)
│   ├── Runtime UI: Debug overlay
│   └── Edit-Time UI: Property inspector, scene hierarchy
│
└── WingedBean.Plugins.WebUI (Blazor - future)
    ├── Runtime UI: Web-based game UI
    └── Edit-Time UI: Web-based editor
```

### Unity Host UI Options

```
UnityHost (Unity Application)
├── UnityHost.Plugins.UGUI
│   ├── Runtime UI: Canvas-based game UI
│   └── Edit-Time UI: Custom editor windows
│
├── UnityHost.Plugins.UIToolkit
│   ├── Runtime UI: Modern UI system
│   └── Edit-Time UI: Editor UI Toolkit
│
└── UnityHost.Plugins.ImGui (optional)
    ├── Runtime UI: Debug overlay
    └── Edit-Time UI: Inspector windows
```

### Godot Host UI Options

```
GodotHost (Godot Application)
├── GodotHost.Plugins.GodotUI
│   ├── Runtime UI: Control nodes
│   └── Edit-Time UI: EditorPlugin UI
│
└── GodotHost.Plugins.ImGui (optional)
    ├── Runtime UI: Debug overlay
    └── Edit-Time UI: Inspector windows
```

## CrossMilo.Contracts.Hosting

**Name:** `CrossMilo.Contracts.Hosting` (KEPT - not renamed)

**Rationale:**
- Directly extends `Microsoft.Extensions.Hosting`
- Maintains consistency with .NET ecosystem naming
- `IService` extends `IHostedService` from `Microsoft.Extensions.Hosting.Abstractions`
- Developers familiar with .NET will immediately understand the relationship
- "Hosting" in .NET context means "application lifecycle management", not "web hosting" or "graphics backend"

**Responsibilities:**

```csharp
namespace Plate.CrossMilo.Contracts.Hosting.App;

/// <summary>
/// Application lifecycle service.
/// Extends IHostedService from Microsoft.Extensions.Hosting.
/// Manages application startup, shutdown, and mode switching.
/// </summary>
public interface IService : IHostedService
{
    /// <summary>Current application mode</summary>
    AppMode CurrentMode { get; }
    
    /// <summary>Switch between runtime and edit-time modes</summary>
    Task SwitchModeAsync(AppMode mode, CancellationToken ct = default);
    
    /// <summary>Application startup</summary>
    Task StartAsync(CancellationToken ct = default);
    
    /// <summary>Application shutdown</summary>
    Task StopAsync(CancellationToken ct = default);
    
    /// <summary>Mode changed event</summary>
    event EventHandler<AppModeChangedEventArgs>? ModeChanged;
}

/// <summary>
/// Application mode enumeration.
/// </summary>
public enum AppMode
{
    /// <summary>Runtime mode - application is running for end-users</summary>
    Runtime,
    
    /// <summary>Edit-time mode - application is being authored/configured</summary>
    EditTime
}
```

## Design Principles

### 1. Console-First, Portable Later

**Strategy:**
- Build and test in console host (WingedBean) first
- Reduces complexity (no Unity/Godot dependencies)
- Faster iteration and debugging
- Once stable, port to Unity/Godot

**Benefits:**
- Simpler development environment
- Easier to test and debug
- No game engine overhead
- Can run in CI/CD pipelines

### 2. Same Contracts, Different Implementations

**Principle:**
- CrossMilo.Contracts.* are platform-agnostic
- Each host provides its own implementations
- Same game logic works everywhere

**Example:**

```csharp
// Contract (platform-agnostic)
namespace Plate.CrossMilo.Contracts.Audio;
public interface IAudioService
{
    Task PlayAsync(string soundId, CancellationToken ct = default);
}

// Console implementation
namespace WingedBean.Plugins.Audio;
public class LibVlcAudioService : IAudioService
{
    // Uses LibVLC for audio playback
}

// Unity implementation
namespace UnityHost.Plugins.Audio;
public class UnityAudioService : IAudioService
{
    // Uses Unity AudioSource
}

// Godot implementation
namespace GodotHost.Plugins.Audio;
public class GodotAudioService : IAudioService
{
    // Uses Godot AudioStreamPlayer
}
```

### 3. Runtime AND Edit-Time Support

**Principle:**
- All contracts work in both modes
- Different implementations for each mode
- Mode switching is seamless

**Example:**

```csharp
// Contract (works in both modes)
namespace Plate.CrossMilo.Contracts.Scene;
public interface ISceneService
{
    Task LoadSceneAsync(string sceneId, CancellationToken ct = default);
}

// Console runtime implementation
namespace WingedBean.Plugins.Scene;
public class RuntimeSceneService : ISceneService
{
    // Loads scene for gameplay
}

// Console edit-time implementation
namespace WingedBean.Plugins.Scene;
public class EditorSceneService : ISceneService
{
    // Loads scene for editing
    // Includes undo/redo, validation, etc.
}
```

## File Structure

### CrossMilo (Contracts)

```
cross-milo/
├── dotnet/framework/src/
│   ├── CrossMilo.Contracts/              ← Base contracts
│   ├── CrossMilo.Contracts.Hosting/      ← Application lifecycle (extends Microsoft.Extensions.Hosting)
│   ├── CrossMilo.Contracts.UI/           ← Generic UI contracts
│   ├── CrossMilo.Contracts.Terminal/     ← Terminal app contracts
│   ├── CrossMilo.Contracts.Audio/        ← Audio contracts
│   ├── CrossMilo.Contracts.Scene/        ← Scene contracts
│   └── CrossMilo.Contracts.*/            ← Other domain contracts
```

### WingedBean (Console Host)

```
winged-bean/
├── dotnet/console/src/
│   ├── host/
│   │   └── ConsoleDungeon.Host/          ← Console application host
│   │       ├── Program.cs                ← Entry point
│   │       ├── AppModeManager.cs         ← Runtime/edit-time mode switching
│   │       └── PluginLoaderHostedService.cs
│   │
│   └── plugins/
│       ├── WingedBean.Plugins.TerminalUI/     ← Terminal.Gui v2
│       │   ├── RuntimeUI/                     ← Game UI
│       │   └── EditorUI/                      ← Editor UI
│       │
│       ├── WingedBean.Plugins.ImGuiUI/        ← ImGui (future)
│       │   ├── RuntimeUI/                     ← Debug overlay
│       │   └── EditorUI/                      ← Inspector, hierarchy
│       │
│       ├── WingedBean.Plugins.Audio/          ← LibVLC audio
│       ├── WingedBean.Plugins.Scene/          ← Scene management
│       └── WingedBean.Plugins.*/              ← Other plugins
```

### Unity Host (Future)

```
unity-host/
├── Assets/
│   ├── Plugins/
│   │   ├── UnityHost.Plugins.UGUI/       ← Unity UGUI
│   │   │   ├── Runtime/                  ← Game UI
│   │   │   └── Editor/                   ← Editor windows
│   │   │
│   │   ├── UnityHost.Plugins.Audio/      ← Unity AudioSource
│   │   ├── UnityHost.Plugins.Scene/      ← Unity SceneManager
│   │   └── UnityHost.Plugins.*/          ← Other plugins
│   │
│   └── CrossMilo/                        ← CrossMilo contracts (imported)
```

### Godot Host (Future)

```
godot-host/
├── addons/
│   ├── GodotHost.Plugins.GodotUI/        ← Godot Control nodes
│   │   ├── runtime/                      ← Game UI
│   │   └── editor/                       ← EditorPlugins
│   │
│   ├── GodotHost.Plugins.Audio/          ← Godot AudioStreamPlayer
│   ├── GodotHost.Plugins.Scene/          ← Godot SceneTree
│   └── GodotHost.Plugins.*/              ← Other plugins
│
└── crossmilo/                            ← CrossMilo contracts (imported)
```

## Migration Path

### Phase 1: Console-First (Current)

1. ✅ Build console host (WingedBean)
2. ✅ Implement Terminal.Gui v2 plugin
3. ✅ Implement core plugins (Audio, Scene, etc.)
4. 🔄 Add runtime mode support
5. 🔄 Add edit-time mode support
6. 🔄 Implement mode switching

### Phase 2: Unity Port (Future)

1. Create Unity host project
2. Import CrossMilo contracts
3. Implement Unity-specific plugins
4. Test same game logic in Unity
5. Add Unity Editor integration

### Phase 3: Godot Port (Future)

1. Create Godot host project
2. Import CrossMilo contracts
3. Implement Godot-specific plugins
4. Test same game logic in Godot
5. Add Godot Editor integration

## Summary

### Key Takeaways

1. **Host ≠ UI Framework**
   - Host = Application runtime (Console, Unity, Godot)
   - UI Framework = Rendering library (Terminal.Gui, ImGui, UGUI)

2. **Runtime AND Edit-Time**
   - All hosts support both modes
   - Console can switch modes at runtime
   - Unity/Godot have built-in editor modes

3. **Console-First Strategy**
   - Develop in console first (simpler)
   - Port to Unity/Godot later (same contracts)
   - Reduces complexity during development

4. **CrossMilo.Contracts.Hosting**
   - Extends Microsoft.Extensions.Hosting (not renamed)
   - Manages application lifecycle
   - Supports mode switching
   - Maintains .NET ecosystem naming consistency

5. **Portability by Design**
   - Same contracts work everywhere
   - Different implementations per host
   - Same game logic, different execution context

### Advantages Over SadConsole

- ✅ **True portability** - Console, Unity, Godot, Web
- ✅ **Flexible UI** - Any UI framework, not just console-style
- ✅ **Edit-time support** - Built into architecture
- ✅ **Plugin-based** - Extensible and modular
- ✅ **Mode switching** - Runtime ↔ Edit-time seamlessly

## References

- **SadConsole:** https://github.com/Thraka/SadConsole
- **CrossMilo Contracts:** `/plate-projects/cross-milo`
- **WingedBean Host:** `/yokan-projects/winged-bean`
- **TerminalUI Refactoring:** `TERMINALUI_REFACTORING.md`
