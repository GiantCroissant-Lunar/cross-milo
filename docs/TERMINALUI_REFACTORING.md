# TerminalUI Refactoring Plan

**Date:** 2025-10-11  
**Status:** Planned  
**Priority:** Medium

## Problem Statement

`CrossMilo.Contracts.TerminalUI` is currently misplaced in the CrossMilo repository. It contains platform-specific Terminal.Gui v2 implementation concerns, which violates the separation between platform-agnostic contracts (CrossMilo) and platform-specific implementations (WingedBean).

### Current Architecture (Incorrect)

```
CrossMilo/
├── CrossMilo.Contracts/              ← Base contracts ✅
├── CrossMilo.Contracts.UI/           ← Platform-agnostic UI contracts ✅
├── CrossMilo.Contracts.Terminal/     ← Terminal hosting contracts ✅
└── CrossMilo.Contracts.TerminalUI/   ← Terminal.Gui v2 specific ❌ WRONG!
```

### Issues

1. **Violates separation of concerns** - CrossMilo should only contain platform-agnostic contracts
2. **Platform-specific implementation in contracts layer** - Terminal.Gui v2 is console-specific
3. **Wrong repository** - Terminal UI implementation belongs in WingedBean (console application)
4. **Confusing naming** - `TerminalUI` sounds like a contract but contains implementation details

## Proposed Solution

Move `CrossMilo.Contracts.TerminalUI` to `WingedBean.Plugins.TerminalUI` in the WingedBean repository.

### Target Architecture (Correct)

```
CrossMilo/ (Platform-Agnostic Contracts)
├── CrossMilo.Contracts/              ← Base contracts (IRegistry, etc.)
├── CrossMilo.Contracts.UI/           ← Generic UI contracts
└── CrossMilo.Contracts.Terminal/     ← Terminal app hosting contracts

WingedBean/ (Console Application - Platform-Specific)
└── dotnet/console/src/plugins/
    └── WingedBean.Plugins.TerminalUI/   ← Terminal.Gui v2 implementation
        ├── TerminalGuiApp.cs            ← Implements ITerminalApp, IUIApp
        ├── TerminalUIService.cs         ← Implements IService
        └── WingedBean.Plugins.TerminalUI.csproj
```

## Migration Steps

### Phase 1: Preparation

1. **Document current dependencies**
   - [ ] List all projects referencing `CrossMilo.Contracts.TerminalUI`
   - [ ] Identify external consumers (if any)
   - [ ] Document public API surface

2. **Create WingedBean plugin structure**
   - [ ] Create `WingedBean.Plugins.TerminalUI` project in WingedBean repo
   - [ ] Set up project references to CrossMilo contracts
   - [ ] Add Terminal.Gui v2 package reference

### Phase 2: Code Migration

3. **Move types to WingedBean**
   - [ ] Move `ITerminalApp` → Keep in `CrossMilo.Contracts.Terminal` (it's a contract)
   - [ ] Move `TerminalAppConfig` → `WingedBean.Plugins.TerminalUI`
   - [ ] Move `TerminalOutputEventArgs` → `WingedBean.Plugins.TerminalUI`
   - [ ] Move `TerminalExitEventArgs` → `WingedBean.Plugins.TerminalUI`
   - [ ] Move `Services/IService` → Implement in `WingedBean.Plugins.TerminalUI`

4. **Update namespaces**
   - [ ] Change `Plate.CrossMilo.Contracts.TerminalUI` → `Plate.WingedBean.Plugins.TerminalUI`
   - [ ] Update all using statements in dependent projects

5. **Update project references**
   - [ ] Remove `CrossMilo.Contracts.TerminalUI` references
   - [ ] Add `WingedBean.Plugins.TerminalUI` references where needed
   - [ ] Add `CrossMilo.Contracts.Terminal` references to plugin

### Phase 3: Implementation

6. **Implement plugin**
   - [ ] Create `TerminalGuiApp : ITerminalApp, IUIApp`
   - [ ] Create `TerminalUIService : IService`
   - [ ] Implement all interface members
   - [ ] Add Terminal.Gui v2 specific logic

7. **Update adapters**
   - [ ] Update `LegacyTerminalAppAdapter` to use new namespace
   - [ ] Verify plugin discovery works correctly
   - [ ] Test plugin registration with `IRegistry`

### Phase 4: Testing & Validation

8. **Test migration**
   - [ ] Build CrossMilo (should succeed)
   - [ ] Build WingedBean (should succeed)
   - [ ] Run all tests
   - [ ] Verify plugin loads correctly at runtime
   - [ ] Test Terminal.Gui v2 functionality

9. **Update documentation**
   - [ ] Update architecture diagrams
   - [ ] Update README files
   - [ ] Document plugin discovery process
   - [ ] Add migration notes for external consumers

### Phase 5: Cleanup

10. **Remove old code**
    - [ ] Delete `CrossMilo.Contracts.TerminalUI` project
    - [ ] Remove from solution file
    - [ ] Update `.gitignore` if needed
    - [ ] Clean up build artifacts

11. **Commit and push**
    - [ ] Commit CrossMilo changes
    - [ ] Commit WingedBean changes
    - [ ] Update CHANGELOG
    - [ ] Create migration guide

## Type Mapping

### Types to Keep in CrossMilo

| Type | Current Location | New Location | Reason |
|------|-----------------|--------------|--------|
| `ITerminalApp` | `CrossMilo.Contracts.TerminalUI` | `CrossMilo.Contracts.Terminal` | It's a contract interface |

### Types to Move to WingedBean

| Type | Current Location | New Location | Reason |
|------|-----------------|--------------|--------|
| `TerminalAppConfig` | `CrossMilo.Contracts.TerminalUI` | `WingedBean.Plugins.TerminalUI` | Configuration for implementation |
| `TerminalOutputEventArgs` | `CrossMilo.Contracts.TerminalUI` | `WingedBean.Plugins.TerminalUI` | Implementation-specific event args |
| `TerminalExitEventArgs` | `CrossMilo.Contracts.TerminalUI` | `WingedBean.Plugins.TerminalUI` | Implementation-specific event args |
| `Services/IService` | `CrossMilo.Contracts.TerminalUI.Services` | `WingedBean.Plugins.TerminalUI` | Terminal.Gui v2 service interface |

## Dependencies After Migration

### CrossMilo.Contracts.Terminal

```xml
<ItemGroup>
  <ProjectReference Include="../CrossMilo.Contracts.UI/CrossMilo.Contracts.UI.csproj" />
</ItemGroup>
```

### WingedBean.Plugins.TerminalUI

```xml
<ItemGroup>
  <!-- CrossMilo contracts -->
  <ProjectReference Include="path/to/CrossMilo.Contracts/CrossMilo.Contracts.csproj" />
  <ProjectReference Include="path/to/CrossMilo.Contracts.UI/CrossMilo.Contracts.UI.csproj" />
  <ProjectReference Include="path/to/CrossMilo.Contracts.Terminal/CrossMilo.Contracts.Terminal.csproj" />
  
  <!-- Terminal.Gui v2 -->
  <PackageReference Include="Terminal.Gui" Version="2.x.x" />
</ItemGroup>
```

## Breaking Changes

### For External Consumers

If any external projects reference `CrossMilo.Contracts.TerminalUI`:

1. **Update project references:**
   ```diff
   - <ProjectReference Include="CrossMilo.Contracts.TerminalUI/CrossMilo.Contracts.TerminalUI.csproj" />
   + <ProjectReference Include="WingedBean.Plugins.TerminalUI/WingedBean.Plugins.TerminalUI.csproj" />
   ```

2. **Update using statements:**
   ```diff
   - using Plate.CrossMilo.Contracts.TerminalUI;
   + using Plate.WingedBean.Plugins.TerminalUI;
   ```

3. **Update interface references:**
   ```diff
   - ITerminalApp (from CrossMilo.Contracts.TerminalUI)
   + ITerminalApp (from CrossMilo.Contracts.Terminal)
   ```

## Rollback Plan

If migration fails:

1. Revert WingedBean commits
2. Revert CrossMilo commits
3. Restore `CrossMilo.Contracts.TerminalUI` from git history
4. Document issues encountered
5. Re-plan migration with lessons learned

## Success Criteria

Migration is complete when:

- ✅ `CrossMilo.Contracts.TerminalUI` project is deleted
- ✅ `WingedBean.Plugins.TerminalUI` project exists and builds
- ✅ All CrossMilo projects build successfully
- ✅ All WingedBean projects build successfully
- ✅ All tests pass
- ✅ Plugin loads and runs correctly in WingedBean
- ✅ Terminal.Gui v2 functionality works as expected
- ✅ No circular dependencies
- ✅ Documentation is updated
- ✅ Changes are committed and pushed

## Timeline

**Estimated effort:** 4-6 hours

- Phase 1: 30 minutes
- Phase 2: 1-2 hours
- Phase 3: 1-2 hours
- Phase 4: 1 hour
- Phase 5: 30 minutes

## References

- **CrossMilo Repository:** `/Users/apprenticegc/Work/lunar-horse/personal-work/plate-projects/cross-milo`
- **WingedBean Repository:** `/Users/apprenticegc/Work/lunar-horse/personal-work/yokan-projects/winged-bean`
- **Current TerminalUI Location:** `cross-milo/dotnet/framework/src/CrossMilo.Contracts.TerminalUI`
- **Target Plugin Location:** `winged-bean/dotnet/console/src/plugins/WingedBean.Plugins.TerminalUI`

## Notes

- This refactoring aligns with the principle: **Contracts in CrossMilo, Implementations in WingedBean**
- Similar pattern should be applied to other platform-specific implementations
- Consider creating a migration guide for future similar refactorings
