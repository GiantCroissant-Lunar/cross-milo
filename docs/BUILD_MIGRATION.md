# Build System Migration

## Overview

Cross-Milo has been migrated from a custom NUKE build project to use **[build-arcane](https://github.com/GiantCroissant-Lunar/build-arcane)**, a config-driven dotnet tool that eliminates the need for per-repo Build.cs files.

## What Changed

### Before (Old NUKE Build)
```
build/nuke/
├── build/
│   ├── Build.cs                        # ~100 lines
│   ├── Build.Configuration.cs          # ~300 lines
│   ├── Build.LocalRepoSync.cs          # ~250 lines
│   ├── Build.NuGetPackaging.cs         # ~200 lines
│   ├── Build.NuGetRepository.cs        # ~200 lines
│   ├── Build.ComponentReporting.cs     # ~700 lines
│   ├── Build.WorkspaceNuGet.cs         # ~250 lines
│   ├── Build.WrapperPath.cs            # ~150 lines
│   ├── _build.csproj                   # NUKE project
│   └── ... (2000+ lines of build code)
├── build-config.json                   # Complex config
├── build.sh                            # Bootstrap script
└── build.ps1                           # Bootstrap script
```

**Total:** ~2000+ lines of build code to maintain

### After (build-arcane)
```
.config/
└── dotnet-tools.json                   # Tool version pin (5 lines)
build-config.json                       # Simplified config (50 lines)
```

**Total:** ~55 lines total, no build code to maintain

## Benefits

✅ **Zero Build Code** - No Build.cs files to maintain
✅ **Version Pinned** - Tool version controlled via `.config/dotnet-tools.json`
✅ **Config-Driven** - All build behavior defined in `build-config.json`
✅ **Centralized Updates** - Build logic updates via tool version bump
✅ **Simpler CI/CD** - Standard `dotnet tool restore && dotnet lunar-build ci`

## Usage

### Install Tool

```bash
# Restore tools (includes build-arcane)
dotnet tool restore
```

### Build Commands

```bash
# Clean artifacts
dotnet lunar-build clean

# Restore NuGet packages
dotnet lunar-build restore

# Compile projects
dotnet lunar-build compile

# Run tests
dotnet lunar-build test

# Create NuGet packages
dotnet lunar-build pack

# Full CI pipeline (clean → restore → compile → test → pack)
dotnet lunar-build ci
```

### Configuration

Edit `build-config.json` to customize build behavior:

```json
{
  "solution": "dotnet/framework/CrossMilo.Contracts.sln",
  "projects": [
    "dotnet/framework/src/CrossMilo.Contracts.*/CrossMilo.Contracts.*.csproj"
  ],
  "tests": {
    "enabled": true,
    "filter": null,
    "coverage": false
  },
  "pack": {
    "enable": true,
    "versioning": "GitVersion"
  }
}
```

## Migration Details

### Projects Included

All 18 CrossMilo.Contracts projects:
- CrossMilo.SourceGenerators.Proxy
- CrossMilo.Contracts.Analytics
- CrossMilo.Contracts.Audio
- CrossMilo.Contracts.Config
- CrossMilo.Contracts.Diagnostics
- CrossMilo.Contracts.ECS
- CrossMilo.Contracts.FigmaSharp
- CrossMilo.Contracts.Game
- CrossMilo.Contracts.Hosting
- CrossMilo.Contracts.Input
- CrossMilo.Contracts.Recorder
- CrossMilo.Contracts.Resilience
- CrossMilo.Contracts.Resource
- CrossMilo.Contracts.Scene
- CrossMilo.Contracts.Terminal
- CrossMilo.Contracts.TerminalUI
- CrossMilo.Contracts.UI
- CrossMilo.Contracts.WebSocket

### Removed

- ❌ `build/nuke/` directory (~2000+ lines of build code)
- ❌ Custom NUKE build project
- ❌ Complex build-config.json with custom schema
- ❌ Build bootstrapper scripts

### Added

- ✅ `.config/dotnet-tools.json` - Tool manifest
- ✅ `build-config.json` - Simplified config (root level)
- ✅ `BUILD_MIGRATION.md` - This document

## Versioning

Build artifacts are now organized by GitVersion:

```
build/_artifacts/
└── 0.0.1-4/
    ├── build-info.txt
    ├── build/
    ├── packages/
    └── test-results/
```

Each build creates a versioned directory with all artifacts.

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Build

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0  # Required for GitVersion

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore tools
        run: dotnet tool restore

      - name: Build
        run: dotnet lunar-build ci --config build-config.json

      - name: Upload artifacts
        uses: actions/upload-artifact@v4
        with:
          name: packages
          path: build/_artifacts/*/packages/*.nupkg
```

## Troubleshooting

### Tool Not Found

```bash
# Restore tools
dotnet tool restore

# Verify installation
dotnet tool list
```

### Config Validation Errors

The tool validates `build-config.json` against a schema. Common issues:

- Missing required fields (`solution`, `projects`)
- Invalid project paths
- Malformed JSON

### Build Failures

Build failures are typically due to:
- Missing NuGet packages (run `dotnet lunar-build restore`)
- Compilation errors in source code (not build system issues)
- Missing dependencies

## Support

- **build-arcane**: https://github.com/GiantCroissant-Lunar/build-arcane
- **Issues**: https://github.com/GiantCroissant-Lunar/build-arcane/issues
- **Documentation**: See build-arcane README

## Version History

- **2025-10-11**: Migrated to build-arcane v0.0.1-19
- **Previous**: Custom NUKE build with Lunar.Build components
