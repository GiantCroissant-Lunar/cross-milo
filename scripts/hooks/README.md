# Git Hooks for Cross-Milo

This directory contains Git hooks for enforcing project standards and quality checks.

## Available Hooks

### pre-commit: .NET Code Analyzer

**Script**: `pre_commit_dotnet_analyzer.py`

**Purpose**: Validates .NET code quality before commits.

**What it checks**:
- Dotnet SDK availability
- Solution builds successfully
- Common anti-patterns (Console.WriteLine in production code, TODO markers)

**Behavior**:
- ✅ **Hard block** on build failures
- ⚠️ **Warnings** for anti-patterns (non-blocking)

## Installation

### Option 1: Using pre-commit Framework (Recommended)

The pre-commit framework provides a standardized way to manage git hooks.

#### Step 1: Install pre-commit

```bash
# Using pip
pip install pre-commit

# OR using pipx (recommended for global install)
pipx install pre-commit

# OR using homebrew (macOS)
brew install pre-commit
```

#### Step 2: Install hooks in repository

```bash
cd /path/to/cross-milo

# Install pre-commit hooks
pre-commit install

# Install commit-msg hook (if needed in future)
pre-commit install --hook-type commit-msg

# Verify installation
pre-commit --version
```

#### Step 3: Test the hooks

```bash
# Run hooks on all files (optional)
pre-commit run --all-files

# Or just make a test commit
git add .
git commit -m "test: verify pre-commit hooks"
```

### Option 2: Manual Installation

If you prefer not to use the pre-commit framework:

```bash
cd /path/to/cross-milo

# Create symlink to pre-commit hook
ln -sf ../../scripts/hooks/pre_commit_dotnet_analyzer.py .git/hooks/pre-commit

# Verify installation
ls -la .git/hooks/pre-commit
chmod +x .git/hooks/pre-commit
```

## Usage

### Normal Workflow

Hooks run automatically on `git commit`:

```bash
git add dotnet/framework/src/SomeFile.cs
git commit -m "feat: add new feature"

# Hook runs and validates code
# 🔍 Running .NET code analyzer for cross-milo...
# ✓ All .NET code validation passed
# [main abc1234] feat: add new feature
```

### Bypass Hook (Emergency Only)

If you need to bypass validation (NOT RECOMMENDED):

```bash
git commit --no-verify -m "Emergency fix"
```

**Warning**: Bypassing the hook may introduce code quality issues.

### Run Hooks Manually

```bash
# Run all hooks on staged files
pre-commit run

# Run all hooks on all files
pre-commit run --all-files

# Run specific hook
pre-commit run dotnet-format --all-files
```

## Configured Hooks

The `.pre-commit-config.yaml` includes:

### File Hygiene
- Trailing whitespace removal
- End-of-file fixer
- JSON/XML/YAML validation
- Merge conflict detection
- Large file detection

### Security
- **gitleaks**: Secret detection

### Python (for hook scripts)
- **ruff**: Linting and formatting

### YAML
- **yamllint**: YAML linting

### Markdown
- **markdownlint**: Markdown structure validation

### .NET
- **dotnet format**: Code formatting for C# files

## Customization

### Adding New Hooks

1. Create a new Python script in `scripts/hooks/`
2. Make it executable: `chmod +x scripts/hooks/your_hook.py`
3. Add entry to `.pre-commit-config.yaml`:

```yaml
- repo: local
  hooks:
    - id: your-custom-hook
      name: Your Custom Hook
      entry: scripts/hooks/your_hook.py
      language: system
      files: \.(cs|fs)$  # File pattern
```

4. Reinstall hooks: `pre-commit install`

### Modifying Existing Hooks

1. Edit the hook script in `scripts/hooks/`
2. Test manually: `pre-commit run your-hook-id --all-files`
3. Commit changes

## Troubleshooting

### Hook doesn't run

Check installation:
```bash
ls -la .git/hooks/
# Should show pre-commit file (symlink or script)

# Reinstall if needed
pre-commit install
```

### "dotnet CLI not found" error

Install .NET SDK:
```bash
# macOS
brew install dotnet

# Or download from: https://dotnet.microsoft.com/download
```

### Hook runs slowly

Skip expensive checks during development:
```bash
# Disable specific hooks temporarily
SKIP=dotnet-format git commit -m "WIP: quick fix"

# Or skip all hooks
git commit --no-verify -m "WIP: quick fix"
```

### Update hooks to latest versions

```bash
# Update all hook repositories to latest versions
pre-commit autoupdate

# Review changes in .pre-commit-config.yaml
git diff .pre-commit-config.yaml
```

## Testing

### Test hook scripts directly

```bash
# Stage some files
git add dotnet/framework/src/*.cs

# Run hook manually
./scripts/hooks/pre_commit_dotnet_analyzer.py
```

### Test with pre-commit framework

```bash
# Run specific hook on all files
pre-commit run dotnet-format --all-files

# Run all hooks on specific file
pre-commit run --files dotnet/framework/src/SomeFile.cs
```

## CI/CD Integration

To ensure hooks are enforced in CI:

```yaml
# .github/workflows/ci.yml
name: CI

on: [push, pull_request]

jobs:
  pre-commit:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-python@v5
        with:
          python-version: '3.11'
      - name: Install pre-commit
        run: pip install pre-commit
      - name: Run pre-commit
        run: pre-commit run --all-files
```

## Related Documentation

- **pre-commit framework**: https://pre-commit.com/
- **pre-commit hooks**: https://pre-commit.com/hooks.html
- **.NET SDK**: https://dotnet.microsoft.com/download

## Maintenance

### Updating Dependencies

Pre-commit dependencies are versioned in `.pre-commit-config.yaml`:

```bash
# Update to latest versions
pre-commit autoupdate

# Commit the changes
git add .pre-commit-config.yaml
git commit -m "chore: update pre-commit hooks"
```

---

**Last Updated**: 2025-10-07
**Version**: 1.0.0 (Initial setup for cross-milo)
