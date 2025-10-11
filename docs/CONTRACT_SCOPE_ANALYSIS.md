# Contract Scope Analysis: ECS, Game, Terminal

**Date:** 2025-10-11  
**Status:** Analysis & Recommendations  
**Issue:** Three contracts may be too specific or implementation-dependent for framework level

## Problem Statement

Three CrossMilo contracts are questionable:

1. **`CrossMilo.Contracts.ECS`** - ECS implementations (Arch, Unity DOTS) are too different to abstract
2. **`CrossMilo.Contracts.Game`** - Game-specific logic doesn't belong in framework
3. **`CrossMilo.Contracts.Terminal`** - Unclear what terminal contracts should do at framework level

## Analysis

### 1. CrossMilo.Contracts.ECS

**Current State:**
- Defines `IECSService`, `IWorld`, `IEntity`, `IQuery`, `IECSSystem`
- Attempts to abstract ECS operations (create world, create entity, add component, etc.)
- Claims to support "Arch ECS, DefaultEcs, Unity ECS, etc."

**Problems:**

#### ❌ ECS Implementations Are Too Different

**Arch ECS:**
```csharp
var world = World.Create();
var entity = world.Create(new Position(0, 0), new Velocity(1, 1));
var query = new QueryDescription().WithAll<Position, Velocity>();
world.Query(in query, (ref Position pos, ref Velocity vel) => {
    pos.X += vel.X;
    pos.Y += vel.Y;
});
```

**Unity DOTS:**
```csharp
var world = new World("MyWorld");
var entityManager = world.EntityManager;
var entity = entityManager.CreateEntity(typeof(Position), typeof(Velocity));
var query = entityManager.CreateEntityQuery(typeof(Position), typeof(Velocity));
Entities.ForEach((ref Position pos, ref Velocity vel) => {
    pos.Value += vel.Value;
}).Schedule();
```

**DefaultEcs:**
```csharp
var world = new World();
var entity = world.CreateEntity();
entity.Set(new Position(0, 0));
entity.Set(new Velocity(1, 1));
world.GetEntities().With<Position>().With<Velocity>().AsSet().GetEntities();
```

**Key Differences:**
- **Component storage** - Arch uses archetypes, Unity uses chunks, DefaultEcs uses sparse sets
- **Query syntax** - Completely different APIs
- **System execution** - Arch uses delegates, Unity uses ISystem, DefaultEcs uses ISystem<T>
- **Performance characteristics** - Different memory layouts, different cache patterns
- **Type safety** - Unity uses Burst compiler, Arch uses C# generics, DefaultEcs uses reflection

**Conclusion:** **You cannot meaningfully abstract ECS implementations.** The abstraction would either be:
1. Too leaky (exposing implementation details)
2. Too restrictive (limiting performance)
3. Too generic (providing no value)

#### ✅ Recommendation: Remove CrossMilo.Contracts.ECS

**Instead:**
- Let each host choose its own ECS library
- Console host: Use Arch ECS directly
- Unity host: Use Unity DOTS directly
- Godot host: Use Godot's node system or a C# ECS library

**Game logic should be ECS-agnostic:**
```csharp
// Instead of exposing ECS details in contracts:
public interface IGameService
{
    void Update(float deltaTime);
    void HandleInput(GameInput input);
    IObservable<GameState> StateObservable { get; }
}

// Implementation can use ANY ECS library internally:
public class ArchECSGameService : IGameService
{
    private World _world; // Arch ECS world
    // ...
}

public class UnityECSGameService : IGameService
{
    private World _world; // Unity DOTS world
    // ...
}
```

### 2. CrossMilo.Contracts.Game

**Current State:**
- Defines `GameInput`, `InputType`, `GameState`, `EntitySnapshot`, `PlayerStats`
- Defines `IDungeonService` - dungeon crawler game logic
- Defines `IRenderService` - game rendering

**Problems:**

#### ❌ Game-Specific Logic Doesn't Belong in Framework

**This is dungeon crawler specific:**
```csharp
public enum InputType
{
    MoveUp, MoveDown, MoveLeft, MoveRight,
    Attack, UseItem, Inventory, Quit
}

public record PlayerStats(
    int CurrentHP, int MaxHP,
    int CurrentMana, int MaxMana,
    int Level, int Experience,
    int Strength, int Dexterity, int Intelligence, int Defense
);
```

**What if you want to make:**
- A platformer game? (Jump, Run, Crouch)
- A racing game? (Accelerate, Brake, Steer)
- A puzzle game? (Select, Rotate, Place)
- A strategy game? (Select Unit, Move, Attack, Build)

**Each game has different:**
- Input types
- Player stats
- Game state
- Entity types
- Rendering requirements

#### ✅ Recommendation: Remove CrossMilo.Contracts.Game

**Instead:**
- Create **game-specific contracts** in your game project
- CrossMilo provides **generic infrastructure** (Input, Audio, Scene, UI)
- Game logic uses CrossMilo infrastructure but defines its own contracts

**Example Structure:**
```
ConsoleDungeon/ (Your specific game)
├── ConsoleDungeon.Contracts/          ← Game-specific contracts
│   ├── IDungeonService.cs             ← Dungeon game logic
│   ├── DungeonTypes.cs                ← Dungeon-specific types
│   └── DungeonEvents.cs               ← Dungeon-specific events
│
├── ConsoleDungeon.Game/               ← Game implementation
│   └── DungeonGameService.cs          ← Uses Arch ECS internally
│
└── ConsoleDungeon.Host/               ← Game host
    └── Program.cs                     ← Wires everything together
```

**CrossMilo provides generic infrastructure:**
```
CrossMilo.Contracts/
├── CrossMilo.Contracts.Input/         ← Generic input (keyboard, mouse, gamepad)
├── CrossMilo.Contracts.Audio/         ← Generic audio (play sound, music)
├── CrossMilo.Contracts.Scene/         ← Generic scene management
├── CrossMilo.Contracts.UI/            ← Generic UI (windows, buttons, text)
└── CrossMilo.Contracts.Hosting/       ← Generic application lifecycle
```

### 3. CrossMilo.Contracts.Terminal

**Current State:**
- Defines `ITerminalApp` - Terminal application interface
- Defines `LegacyTerminalAppAdapter` - Adapter for hosting
- Contains event args for terminal output/exit

**Problems:**

#### ❓ What Does "Terminal Contract" Mean?

**Current `ITerminalApp`:**
```csharp
public interface ITerminalApp : IUIApp
{
    Task SendRawInputAsync(byte[] data, CancellationToken ct = default);
    Task SetCursorPositionAsync(int x, int y, CancellationToken ct = default);
    Task WriteAnsiAsync(string ansiSequence, CancellationToken ct = default);
    event EventHandler<TerminalOutputEventArgs>? OutputReceived;
    event EventHandler<TerminalExitEventArgs>? Exited;
}
```

**Questions:**
- Is this for **PTY/pseudo-terminal** functionality? (node-pty, xterm.js)
- Is this for **terminal UI** functionality? (Terminal.Gui, ncurses)
- Is this for **terminal emulation**? (VT100, ANSI escape codes)

#### Analysis of Current Usage

Looking at the code:
- `ITerminalApp` extends `IUIApp` (UI application)
- Has PTY-specific methods (`SendRawInputAsync`, `WriteAnsiAsync`)
- Has terminal-specific events (`OutputReceived`, `Exited`)

**This seems to be for PTY/terminal emulation, not terminal UI.**

#### ✅ Recommendation: Clarify or Remove CrossMilo.Contracts.Terminal

**Option 1: Keep for PTY/Terminal Emulation**

If you're building a **terminal emulator** or **PTY host**:
```
CrossMilo.Contracts.Terminal/
├── ITerminalEmulator.cs               ← Terminal emulation (VT100, ANSI)
├── IPseudoTerminal.cs                 ← PTY functionality (node-pty)
└── TerminalTypes.cs                   ← Terminal-specific types
```

**Use case:** Building xterm.js-like functionality in .NET

**Option 2: Remove if Not Needed**

If you're just building a **console application with Terminal.Gui**:
- You don't need terminal contracts
- Terminal.Gui is just a UI framework (like WPF or Avalonia)
- Use `CrossMilo.Contracts.UI` instead

**Current `LegacyTerminalAppAdapter` should move to:**
```
WingedBean.Host/
└── Adapters/
    └── TerminalAppAdapter.cs          ← Host-specific adapter
```

## Recommendations Summary

### ❌ Remove: CrossMilo.Contracts.ECS

**Reason:** ECS implementations are too different to abstract meaningfully

**Instead:**
- Each host chooses its own ECS library
- Game logic exposes high-level contracts (not ECS details)
- Console: Arch ECS
- Unity: Unity DOTS
- Godot: Godot nodes or custom ECS

### ❌ Remove: CrossMilo.Contracts.Game

**Reason:** Game-specific logic doesn't belong in framework

**Instead:**
- Create game-specific contracts in your game project
- CrossMilo provides generic infrastructure (Input, Audio, Scene, UI)
- Example: `ConsoleDungeon.Contracts` for dungeon crawler

### ⚠️ Clarify or Remove: CrossMilo.Contracts.Terminal

**Reason:** Unclear purpose - PTY emulation or terminal UI?

**Options:**
1. **Keep if building terminal emulator** - Rename to `CrossMilo.Contracts.PseudoTerminal`
2. **Remove if just using Terminal.Gui** - Use `CrossMilo.Contracts.UI` instead

## Revised CrossMilo Structure

### Framework Contracts (Generic Infrastructure)

```
CrossMilo.Contracts/
├── CrossMilo.Contracts/                ← Base contracts (IRegistry, etc.)
├── CrossMilo.Contracts.Hosting/        ← Application lifecycle
├── CrossMilo.Contracts.UI/             ← Generic UI (windows, buttons, text)
├── CrossMilo.Contracts.Input/          ← Generic input (keyboard, mouse, gamepad)
├── CrossMilo.Contracts.Audio/          ← Generic audio (play sound, music)
├── CrossMilo.Contracts.Scene/          ← Generic scene management
├── CrossMilo.Contracts.Resource/       ← Generic resource loading
├── CrossMilo.Contracts.Config/         ← Generic configuration
├── CrossMilo.Contracts.Diagnostics/    ← Generic diagnostics
├── CrossMilo.Contracts.Analytics/      ← Generic analytics
└── CrossMilo.Contracts.Resilience/     ← Generic resilience (retry, circuit breaker)
```

**Principle:** Framework provides **generic, reusable infrastructure**

### Game-Specific Contracts (Your Game)

```
ConsoleDungeon/ (Your specific game project)
├── ConsoleDungeon.Contracts/           ← Game-specific contracts
│   ├── IDungeonService.cs              ← Dungeon game logic
│   ├── IInventoryService.cs            ← Inventory management
│   ├── IBattleService.cs               ← Battle system
│   ├── IQuestService.cs                ← Quest system
│   ├── DungeonTypes.cs                 ← Dungeon-specific types
│   └── DungeonEvents.cs                ← Dungeon-specific events
│
├── ConsoleDungeon.Game/                ← Game implementation
│   ├── DungeonGameService.cs           ← Uses Arch ECS internally
│   ├── InventorySystem.cs              ← Inventory logic
│   ├── BattleSystem.cs                 ← Battle logic
│   └── QuestSystem.cs                  ← Quest logic
│
└── ConsoleDungeon.Host/                ← Game host
    └── Program.cs                      ← Wires everything together
```

**Principle:** Game defines its own contracts and uses framework infrastructure

## Migration Plan

### Phase 1: Remove CrossMilo.Contracts.ECS

1. **Move ECS-specific code to game project:**
   ```
   CrossMilo.Contracts.ECS/ → ConsoleDungeon.Game/ECS/
   ```

2. **Update game service to hide ECS details:**
   ```csharp
   // Before (leaky abstraction):
   public interface IDungeonService
   {
       IWorld World { get; }  // ❌ Exposes ECS details
   }

   // After (clean abstraction):
   public interface IDungeonService
   {
       void Update(float deltaTime);
       void HandleInput(GameInput input);
       IObservable<GameState> StateObservable { get; }
   }
   ```

3. **Implementation uses Arch ECS internally:**
   ```csharp
   public class DungeonGameService : IDungeonService
   {
       private World _world; // Arch ECS (internal detail)
       // ...
   }
   ```

### Phase 2: Remove CrossMilo.Contracts.Game

1. **Create game-specific contracts:**
   ```
   ConsoleDungeon.Contracts/
   ├── IDungeonService.cs
   ├── DungeonTypes.cs (GameInput, GameState, etc.)
   └── DungeonEvents.cs
   ```

2. **Move game-specific types:**
   ```
   CrossMilo.Contracts.Game/GameTypes.cs → ConsoleDungeon.Contracts/DungeonTypes.cs
   CrossMilo.Contracts.Game/Dungeon/ → ConsoleDungeon.Contracts/
   ```

3. **Update references:**
   ```csharp
   // Before:
   using Plate.CrossMilo.Contracts.Game;

   // After:
   using ConsoleDungeon.Contracts;
   ```

### Phase 3: Clarify CrossMilo.Contracts.Terminal

**Option A: Keep for PTY Emulation**
1. Rename to `CrossMilo.Contracts.PseudoTerminal`
2. Document PTY/terminal emulation use case
3. Keep `ITerminalApp` for PTY functionality

**Option B: Remove if Not Needed**
1. Move `LegacyTerminalAppAdapter` to `WingedBean.Host/Adapters/`
2. Remove `CrossMilo.Contracts.Terminal`
3. Use `CrossMilo.Contracts.UI` for UI concerns

## Benefits of This Approach

### ✅ 1. Clear Separation of Concerns

- **Framework** = Generic, reusable infrastructure
- **Game** = Specific game logic and contracts

### ✅ 2. No Leaky Abstractions

- ECS details stay in implementation
- Game logic exposes clean, high-level contracts
- UI doesn't know about ECS

### ✅ 3. True Portability

- Console host uses Arch ECS
- Unity host uses Unity DOTS
- Godot host uses Godot nodes
- **Same game logic contracts, different implementations**

### ✅ 4. Easier to Understand

- Framework contracts are generic and obvious
- Game contracts are specific and clear
- No confusion about what belongs where

### ✅ 5. Better Reusability

- Framework can be used for ANY game
- Not tied to dungeon crawler specifics
- Not tied to specific ECS library

## Example: Building a Platformer Game

With the revised structure:

```
PlatformerGame/
├── PlatformerGame.Contracts/           ← Game-specific contracts
│   ├── IPlatformerService.cs           ← Platformer game logic
│   ├── PlatformerTypes.cs              ← Jump, Run, Crouch, etc.
│   └── PlatformerEvents.cs             ← Level complete, player died, etc.
│
├── PlatformerGame.Game/                ← Game implementation
│   └── PlatformerGameService.cs        ← Uses Arch ECS internally
│
└── PlatformerGame.Host/                ← Game host
    └── Program.cs                      ← Uses CrossMilo infrastructure
```

**CrossMilo infrastructure is reused:**
- `CrossMilo.Contracts.Input` - Keyboard, gamepad input
- `CrossMilo.Contracts.Audio` - Sound effects, music
- `CrossMilo.Contracts.Scene` - Level loading
- `CrossMilo.Contracts.UI` - Menus, HUD

**Game-specific logic is separate:**
- `PlatformerTypes.cs` - Jump, Run, Crouch (not MoveUp, Attack, Inventory)
- `IPlatformerService` - Platformer game logic (not dungeon crawler)

## Conclusion

### Remove These Contracts:

1. ❌ **CrossMilo.Contracts.ECS** - Too implementation-specific
2. ❌ **CrossMilo.Contracts.Game** - Too game-specific

### Clarify This Contract:

3. ⚠️ **CrossMilo.Contracts.Terminal** - Clarify purpose or remove

### Keep These Contracts (Generic Infrastructure):

✅ **CrossMilo.Contracts.Hosting** - Application lifecycle  
✅ **CrossMilo.Contracts.UI** - Generic UI  
✅ **CrossMilo.Contracts.Input** - Generic input  
✅ **CrossMilo.Contracts.Audio** - Generic audio  
✅ **CrossMilo.Contracts.Scene** - Generic scene management  
✅ **CrossMilo.Contracts.Resource** - Generic resource loading  
✅ **CrossMilo.Contracts.Config** - Generic configuration  
✅ **CrossMilo.Contracts.Diagnostics** - Generic diagnostics  
✅ **CrossMilo.Contracts.Analytics** - Generic analytics  
✅ **CrossMilo.Contracts.Resilience** - Generic resilience  

### Create Game-Specific Contracts:

✅ **ConsoleDungeon.Contracts** - Dungeon crawler contracts  
✅ **ConsoleDungeon.Game** - Dungeon crawler implementation (uses Arch ECS internally)  

This approach gives you:
- **Clean framework** - Generic, reusable infrastructure
- **Clean game** - Specific game logic and contracts
- **True portability** - Same contracts, different implementations
- **No leaky abstractions** - ECS details stay hidden
- **Easy to understand** - Clear separation of concerns

## Next Steps

1. Review this analysis
2. Decide on CrossMilo.Contracts.Terminal (keep for PTY or remove)
3. Create migration plan for removing ECS and Game contracts
4. Create ConsoleDungeon.Contracts project
5. Move game-specific code to ConsoleDungeon
6. Update documentation

Your instincts were correct - these contracts don't belong at the framework level! 🎯
