# Hook-Validator Integration

## Overview

Cross-Milo has integrated **[hook-validator](https://github.com/GiantCroissant-Lunar/hook-validator)** for enhanced .NET-specific validation in pre-commit hooks.

## What Changed

### Before (Generic Pre-commit Hooks)
```yaml
- repo: local
  hooks:
    - id: dotnet-format
      # Only formatting, no validation
```

**Limitations:**
- ❌ No namespace validation
- ❌ No .csproj property validation
- ❌ No TODO format enforcement
- ❌ Generic error messages

### After (hook-validator Integration)
```yaml
- repo: local
  hooks:
    - id: check-namespace
      # Validates C# namespaces match directory structure
      args: [--namespace-prefix=Plate.CrossMilo]
      
    - id: validate-csproj
      # Validates .csproj files have required properties
      
    - id: validate-todo-format
      # Enforces TODO(author): description format
      
    - id: dotnet-format
      # Still includes formatting
```

**Benefits:**
- ✅ Namespace validation with helpful hints
- ✅ .csproj property validation
- ✅ TODO format enforcement
- ✅ Actionable error messages
- ✅ Catches issues before commit

## Validators Added

### 1. C# Namespace Validation

**Purpose:** Ensures namespaces match directory structure

**Configuration:**
```yaml
- id: check-namespace
  args: [--namespace-prefix=Plate.CrossMilo]
```

**Example Error:**
```
dotnet/framework/src/CrossMilo.Contracts.Terminal/Terminal.cs: Namespace mismatch
  Expected: Plate.CrossMilo.Contracts.Terminal
  Actual:   WingedBean.Contracts.Terminal
  Hint: Update namespace to match directory structure
```

**Issues Found:**
- Several files have incorrect namespace prefixes
- Some files use `WingedBean.*` instead of `Plate.CrossMilo.*`
- Subdirectory namespaces don't always match structure

### 2. .csproj Validation

**Purpose:** Validates .csproj files have required properties and package versions

**Configuration:**
```yaml
- id: validate-csproj
  files: \.csproj$
```

**Example Error:**
```
CrossMilo.Contracts.ECS.csproj:
  - PackageReference 'PolySharp' missing version
  
CrossMilo.Contracts.Hosting.csproj:
  - PackageReference 'Microsoft.Extensions.Hosting.Abstractions' missing version
  - PackageReference 'Microsoft.Extensions.DependencyInjection.Abstractions' missing version
```

**Issues Found:**
- Multiple projects missing package versions
- `PolySharp` package used without version in 10+ projects
- `Microsoft.Extensions.*` packages missing versions in several projects

**Note:** This is likely due to Central Package Management (CPM). The validator needs to be updated to support CPM's `Directory.Packages.props` pattern.

### 3. TODO Format Validation

**Purpose:** Enforces consistent TODO comment format

**Configuration:**
```yaml
- id: validate-todo-format
  types: [text]
```

**Expected Format:**
```
// TODO(author): description
// TODO(author, #123): description with issue link
```

**Example Errors:**
```
scripts/hooks/pre_commit_dotnet_analyzer.py:91: Invalid TODO format
  Found: if "TODO: Implement" in content:
  Expected: TODO(author): description or TODO(author, #issue): description
```

**Issues Found:**
- Several TODOs without author attribution
- Some TODOs in comments that aren't actual TODOs (e.g., "TODO markers")

## Installation

Hook-validator is installed in development mode:

```bash
pip install -e ../hook-validator
```

This allows using the latest validators without publishing to PyPI.

## Usage

### Run All Hooks

```bash
pre-commit run --all-files
```

### Run Specific Validator

```bash
# Namespace validation only
pre-commit run check-namespace --all-files

# .csproj validation only
pre-commit run validate-csproj --all-files

# TODO format validation only
pre-commit run validate-todo-format --all-files
```

### Skip Hooks (When Needed)

```bash
# Skip all hooks
git commit --no-verify

# Skip specific hook
SKIP=check-namespace git commit
```

## Issues Found Summary

### High Priority

1. **Namespace Mismatches** (Multiple files)
   - Some files use `WingedBean.*` instead of `Plate.CrossMilo.*`
   - Subdirectory namespaces don't match structure
   - **Action:** Update namespaces to match directory structure

2. **Missing Package Versions** (10+ projects)
   - `PolySharp` missing version in multiple projects
   - `Microsoft.Extensions.*` packages missing versions
   - **Action:** Add versions to `Directory.Packages.props` or individual .csproj files

### Medium Priority

3. **TODO Format Issues** (5+ files)
   - TODOs without author attribution
   - Inconsistent TODO format
   - **Action:** Update TODOs to follow `TODO(author): description` format

## Known Limitations

### Central Package Management (CPM)

The `.csproj` validator currently reports missing versions for packages managed by CPM via `Directory.Packages.props`. This is a false positive.

**Workaround Options:**
1. Exclude CPM-managed projects from validation
2. Update validator to support CPM (check for `Directory.Packages.props`)
3. Accept warnings and focus on actual issues

**Recommended:** Update hook-validator to support CPM pattern.

## Future Enhancements

### When hook-validator is Published

Replace `repo: local` with published version:

```yaml
- repo: https://github.com/GiantCroissant-Lunar/hook-validator
  rev: v1.0.0
  hooks:
    - id: check-namespace
      args: [--namespace-prefix=Plate.CrossMilo]
    - id: validate-csproj
    - id: validate-todo-format
```

### Additional Validators

Consider adding:
- `check-solution-structure` - Validate solution organization
- `check-file-naming` - Enforce PascalCase for C# files

## Configuration File

Create `.lunar-validators.yaml` for project-specific config:

```yaml
dotnet:
  namespace_prefix: Plate.CrossMilo
  required_properties:
    - TargetFramework
    - Nullable
    - ImplicitUsings
  solution_structure:
    - dotnet/framework/src/
    - dotnet/framework/tests/
    
common:
  todo_format: "TODO(author): description"
  file_naming: PascalCase
```

## Benefits Summary

| Aspect | Before | After |
|--------|--------|-------|
| **Namespace Validation** | None | ✅ Full validation |
| **.csproj Validation** | None | ✅ Property checks |
| **TODO Format** | Inconsistent | ✅ Enforced |
| **Error Messages** | Generic | ✅ Actionable hints |
| **Issue Detection** | Manual | ✅ Automatic |

## Support

- **hook-validator**: https://github.com/GiantCroissant-Lunar/hook-validator
- **Issues**: https://github.com/GiantCroissant-Lunar/hook-validator/issues
- **Documentation**: See hook-validator README

## Version History

- **2025-10-11**: Integrated hook-validator v1.0.0 (development mode)
- **Previous**: Generic pre-commit hooks only
