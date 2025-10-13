# CrossMilo Project Overrides

**Project**: CrossMilo.Contracts  
**Stack**: .NET  
**Date**: 2025-10-13

## Project-Specific Rules

### CrossMilo Architecture
- **Plugin System**: Uses source generators for proxy generation
- **Contracts Pattern**: All interfaces in separate contract assemblies
- **Module Organization**: Organized by domain (Analytics, Audio, Config, etc.)

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

### R-CROSSMILO-030: Interface Naming
- **What**: All contract interfaces start with `I`
- **Why**: Standard .NET convention, clear intent
- **Example**: `IAudioService`, `IAnalyticsProvider`

## Modified Base Rules

### R-DOTNET-STRUCT-010: Solution Organization (Modified)
```
cross-milo/
  dotnet/
    framework/
      src/
        CrossMilo.SourceGenerators.Proxy/
        CrossMilo.Contracts/
        CrossMilo.Contracts.{Domain}/
      tests/
        CrossMilo.Contracts.Tests/
  docs/
  build-config.json
```

### R-TEST-020: Test Organization (Modified)
- **What**: Tests are in `dotnet/framework/tests/`
- **Why**: Matches the multi-framework structure
- **Pattern**: `CrossMilo.Contracts.{Domain}.Tests`

## Disabled Rules

None currently disabled.

## Build System

### R-CROSSMILO-BUILD-010: Use build-arcane
- **What**: Project uses build-arcane for builds
- **Config**: `build-config.json` at root
- **Command**: `dotnet lunar-build ci`

### R-CROSSMILO-BUILD-020: Metrics Enabled
- **What**: Code metrics collection is enabled
- **Why**: Track code quality over time
- **Thresholds**:
  - Max average complexity: 50
  - Min comment percentage: 10

## Validation

### R-CROSSMILO-VALID-010: Use hook-validator
- **What**: Pre-commit hooks via hook-validator
- **Config**: `.pre-commit-config.yaml`
- **Hooks**: format-dotnet, check-debug-statements

## Notes

- This project is part of the plate-projects ecosystem
- Uses GitVersion for versioning
- Follows conventional commit messages
- All contracts are interfaces only (no implementation)
