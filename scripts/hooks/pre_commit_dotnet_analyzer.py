#!/usr/bin/env python3
"""
Pre-commit hook: Validate .NET code quality for cross-milo.

This hook validates:
1. .NET solution files can be built
2. Code style compliance (optional)
3. Common anti-patterns
"""

import subprocess
import sys
from pathlib import Path
from typing import List


class DotnetAnalyzer:
    """Validates .NET code quality."""

    def __init__(self):
        self.errors: List[str] = []
        self.warnings: List[str] = []

    def check_dotnet_available(self) -> bool:
        """Check if dotnet CLI is available."""
        try:
            subprocess.run(
                ["dotnet", "--version"],
                capture_output=True,
                text=True,
                check=True,
            )
            return True
        except (subprocess.CalledProcessError, FileNotFoundError):
            self.errors.append("dotnet CLI not found. Please install .NET SDK.")
            return False

    def run_dotnet_build(self, solution_path: Path) -> bool:
        """
        Run dotnet build to verify code compiles.

        Args:
            solution_path: Path to .sln file

        Returns:
            True if build succeeds, False otherwise
        """
        try:
            print(f"  Building {solution_path.name}...", end=" ")
            result = subprocess.run(
                ["dotnet", "build", str(solution_path), "--no-restore", "-v", "quiet"],
                capture_output=True,
                text=True,
                cwd=solution_path.parent,
            )

            if result.returncode == 0:
                print("✓")
                return True
            else:
                print("✗")
                self.errors.append(
                    f"Build failed for {solution_path}:\n{result.stderr}"
                )
                return False

        except subprocess.CalledProcessError as e:
            self.errors.append(f"Failed to build {solution_path}: {e.stderr}")
            return False

    def validate_cs_files(self, cs_files: List[Path]) -> bool:
        """
        Basic validation for C# files.

        Args:
            cs_files: List of .cs files to validate

        Returns:
            True if all files valid, False otherwise
        """
        all_valid = True

        for cs_file in cs_files:
            if not cs_file.exists():
                continue

            try:
                content = cs_file.read_text(encoding="utf-8")

                # Check for common issues
                if "TODO: Implement" in content:
                    self.warnings.append(
                        f"{cs_file}: Contains 'TODO: Implement' marker"
                    )

                # Check for Console.WriteLine in production code (not in tests)
                if "Console.WriteLine" in content and "Tests" not in str(cs_file):
                    self.warnings.append(
                        f"{cs_file}: Contains Console.WriteLine (use ILogger instead)"
                    )

            except Exception as e:
                self.errors.append(f"{cs_file}: Failed to read file: {e}")
                all_valid = False

        return all_valid


def get_staged_files() -> List[Path]:
    """
    Get list of staged files from git.

    Returns:
        List of Path objects for staged files
    """
    try:
        result = subprocess.run(
            ["git", "diff", "--cached", "--name-only", "--diff-filter=ACM"],
            capture_output=True,
            text=True,
            check=True,
        )

        files = [
            Path(f.strip()) for f in result.stdout.strip().split("\n") if f.strip()
        ]

        return files
    except subprocess.CalledProcessError as e:
        print(f"Error getting staged files: {e.stderr}", file=sys.stderr)
        return []


def should_validate_file(file_path: Path) -> bool:
    """
    Check if file should be validated.

    Args:
        file_path: Path to check

    Returns:
        True if file should be validated
    """
    # Validate C# source files
    if file_path.suffix == ".cs":
        return True

    return False


def main() -> int:
    """Main pre-commit hook logic."""
    print("🔍 Running .NET code analyzer for cross-milo...\n")

    analyzer = DotnetAnalyzer()

    # Check dotnet availability
    if not analyzer.check_dotnet_available():
        print("❌ .NET SDK not found")
        for error in analyzer.errors:
            print(f"  • {error}")
        return 1

    # Get staged files
    staged_files = get_staged_files()

    if not staged_files:
        print("No files to validate.")
        return 0

    # Filter C# files
    cs_files = [f for f in staged_files if should_validate_file(f)]

    if not cs_files:
        print(
            f"Scanned {len(staged_files)} staged files, none require .NET validation."
        )
        return 0

    print(f"Validating {len(cs_files)} C# file(s)...\n")

    # Validate CS files
    analyzer.validate_cs_files(cs_files)

    # Try to find and build solution
    repo_root = Path.cwd()
    solution_files = list(repo_root.glob("dotnet/**/*.sln"))

    if solution_files:
        print("\nBuilding solution(s) to verify compilation...\n")
        for sln in solution_files:
            analyzer.run_dotnet_build(sln)

    # Print warnings (non-blocking)
    if analyzer.warnings:
        print("\n⚠️  Warnings:")
        for warning in analyzer.warnings:
            print(f"  • {warning}")

    # Print errors (blocking)
    if analyzer.errors:
        print("\n❌ Validation Errors:")
        for error in analyzer.errors:
            print(f"  • {error}")
        print()
        print("━" * 70)
        print("❌ COMMIT BLOCKED: .NET validation failed")
        print("━" * 70)
        print("\nTo bypass this check (NOT RECOMMENDED): git commit --no-verify")
        return 1

    print("\n✓ All .NET code validation passed")
    return 0


if __name__ == "__main__":
    sys.exit(main())
