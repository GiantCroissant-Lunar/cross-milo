# CrossMilo Project Rules

**Project**: CrossMilo.Contracts  
**Stack**: .NET  
**Type**: Framework (Plate Project)  
**Date**: 2025-10-14

> ⚠️ **Read workspace rules first**: `../../../.agent/local/overrides.md` (or see `workspace-reference.md`)

---

## Project Purpose

CrossMilo provides cross-platform abstraction contracts for the Dungeon game engine. It defines interfaces and contracts that enable plugin-based architecture across multiple domains (Audio, Analytics, Config, Diagnostics, etc.).

**Key Responsibilities:**
- Define domain-specific contract interfaces
- Provide source generators for proxy generation
- Enable plugin isolation through contract assemblies
- Support cross-platform functionality abstraction

---

## Architecture Overview

```
CrossMilo.Contracts/                     # Core contracts
CrossMilo.Contracts.{Domain}/           # Domain-specific contracts
  - Audio                               # Audio system contracts
  - Analytics                           # Analytics contracts
  - Config                              # Configuration contracts
  - Diagnostics                         # Diagnostics contracts
  - Hosting                             # Hosting contracts
  - Input                               # Input system contracts
  - Recorder                            # Recording contracts
  - Resilience                          # Resilience contracts
  - Resource                            # Resource management
  - Scene                               # Scene management
  - Terminal                            # Terminal UI contracts
  - UI                                  # General UI contracts

CrossMilo.SourceGenerators.Proxy/       # Code generation
```

---

## Project-Specific Rules

### R-CROSSMILO-010: Contract Assembly Pattern
- **What**: Each domain has its own contract assembly
- **Why**: Enables plugin isolation and reduces coupling
- **Pattern**: `CrossMilo.Contracts.{Domain}`
- **Examples**: 
  - `CrossMilo.Contracts.Audio`
  - `CrossMilo.Contracts.Analytics`
  - `CrossMilo.Contracts.Hosting`

### R-CROSSMILO-020: Source Generator Usage
- **What**: Use `CrossMilo.SourceGenerators.Proxy` for proxy generation
- **Why**: Reduces boilerplate and ensures consistency
- **Location**: `CrossMilo.SourceGenerators.Proxy` project
- **Usage**: Add as analyzer reference to projects needing proxies

### R-CROSSMILO-030: Interface Naming
- **What**: All contract interfaces start with `I`
- **Why**: Standard .NET convention, clear intent
- **Examples**: `IAudioService`, `IAnalyticsProvider`, `IHostBuilder`

### R-CROSSMILO-040: Contract Immutability
- **What**: Contracts should be stable once published
- **Why**: Breaking changes affect all consumers
- **Pattern**: 
  - Add new interfaces rather than modify existing
  - Use semantic versioning for breaking changes
  - Document all contract changes in CHANGELOG

---

## Project Structure

### R-CROSSMILO-STRUCT-010: Solution Organization
```
cross-milo/
  dotnet/
    framework/
      src/
        CrossMilo.SourceGenerators.Proxy/
        CrossMilo.Contracts/                    # Core contracts
        CrossMilo.Contracts.{Domain}/           # Domain contracts
      tests/
        CrossMilo.Contracts.Tests/
  docs/
    architecture/
    contracts/
  build-config.json
  .config/
    dotnet-tools.json                           # build-arcane reference
```

### R-CROSSMILO-STRUCT-020: Namespace Conventions
- **Pattern**: `GiantCroissant.CrossMilo.Contracts.{Domain}`
- **Examples**:
  - `GiantCroissant.CrossMilo.Contracts.Audio`
  - `GiantCroissant.CrossMilo.Contracts.Hosting`

---

## Build & Validation

### R-CROSSMILO-BUILD-010: Use build-arcane
- **What**: Project uses build-arcane for builds
- **Config**: `build-config.json` at root
- **Command**: `dotnet lunar-build ci`
- **Tool**: Defined in `.config/dotnet-tools.json`

### R-CROSSMILO-BUILD-020: Metrics Enabled
- **What**: Code metrics collection is enabled
- **Why**: Track code quality over time
- **Thresholds**:
  - Max average complexity: 50
  - Min comment percentage: 10

### R-CROSSMILO-VALID-010: Use hook-validator
- **What**: Pre-commit hooks via hook-validator (plate-validators)
- **Config**: `.pre-commit-config.yaml`
- **Hooks**: 
  - format-dotnet
  - check-debug-statements
  - check-namespaces
  - validate-csproj

---

## Testing

### R-CROSSMILO-TEST-010: Test Organization
- **Location**: `dotnet/framework/tests/CrossMilo.Contracts.Tests/`
- **Framework**: xUnit
- **Pattern**: Test contracts in isolation
- **Coverage**: Focus on source generator output validation

### R-CROSSMILO-TEST-020: Contract Testing
- **What**: Test generated proxies match contracts
- **Why**: Ensure source generators produce correct code
- **Pattern**: Snapshot testing for generated code

---

## Dependencies

### Key Dependencies
- **.NET 8**: Target framework
- **build-arcane**: Build automation
- **hook-validator**: Pre-commit validation
- **Microsoft.CodeAnalysis**: For source generators

### Dependents
- **plugin-manoi**: Uses CrossMilo contracts for plugin system
- **host-land**: Implements hosting contracts
- **winged-bean**: Consumes all contracts

---

## Notes

- This is a **framework project** (Plate), not an application
- Changes here affect multiple downstream projects
- Coordinate contract changes with plugin-manoi and host-land teams
- See workspace rules for build system usage (R-LUNARHORSE-030)
