# Golden Recordings

This directory contains reference recordings used for regression testing.

## Purpose

Golden recordings serve as baseline references to detect unintended changes in application behavior. When tests run, new recordings are compared against these golden recordings to ensure consistency.

## Directory Structure

```
golden/
├── desktop/          # Reference recordings for desktop app (SadConsole)
│   └── *.urf.jsonl   # Desktop golden recordings
├── console/          # Reference recordings for console app (Terminal)
│   └── *.urf.jsonl   # Console golden recordings
└── README.md         # This file
```

## Creating Golden Recordings

### Desktop App

```bash
cd personal-work/yokan-projects/winged-bean/development/dotnet/windows
dotnet run --project src/host/Dungeon.Host.SadConsole/Dungeon.Host.SadConsole.csproj

# In the window:
# Press F9 to start recording
# Perform the test scenario (e.g., basic movement, combat, etc.)
# Press F10 to stop recording
# Exit

# Copy to golden directory with descriptive name:
cp src/host/Dungeon.Host.SadConsole/bin/Debug/net8.0/recordings/urf/session-*.urf.jsonl \
   ../../../../../plate-projects/cross-milo/dotnet/tests/test-data/golden/desktop/basic-movement.urf.jsonl
```

### Console App

```bash
cd personal-work/yokan-projects/winged-bean/development/dotnet/console
dotnet run --project src/host/Dungeon.Host.Console/Dungeon.Host.Console.csproj

# Perform the test scenario
# Exit

# Copy to golden directory:
cp src/host/Dungeon.Host.Console/bin/Debug/net8.0/recordings/urf/session-*.urf.jsonl \
   ../../../../../plate-projects/cross-milo/dotnet/tests/test-data/golden/console/basic-movement.urf.jsonl
```

## Naming Convention

Golden recordings should have descriptive names that indicate the scenario:

- `basic-movement.urf.jsonl` - Basic arrow key movement
- `combat-scenario.urf.jsonl` - Combat sequence
- `level-progression.urf.jsonl` - Leveling up
- `startup-sequence.urf.jsonl` - App initialization
- `menu-navigation.urf.jsonl` - Menu interactions

## Using Golden Recordings in Tests

```csharp
[Fact]
public void Recording_MatchesGolden_BasicMovement()
{
    // Arrange
    var golden = URFReader.Load("test-data/golden/desktop/basic-movement.urf.jsonl");
    var actual = RecordScenario("basic-movement"); // Your test harness
    
    // Act
    var comparer = new RecordingComparer();
    var result = comparer.Compare(actual, golden);
    
    // Assert
    Assert.True(result.IsEquivalent, 
        $"Recording differs from golden: {result.Summary}");
}
```

## Tolerance Configuration

The comparison allows for natural variations:

```csharp
var options = new ComparisonOptions
{
    TimestampTolerance = 0.1,      // 100ms timestamp differences
    EventCountTolerance = 0.1,     // 10% event count variation
    DurationTolerance = 0.15,      // 15% duration variation
    IgnoreMetadataVariations = true // Ignore IDs, timestamps in metadata
};

var comparer = new RecordingComparer(options);
```

## When to Update Golden Recordings

Update golden recordings when:

1. **Intentional Behavior Change**: You've made a deliberate change to app behavior
2. **Performance Improvement**: Optimization changes frame rates or timing
3. **UI Update**: Visual changes that affect output
4. **Feature Addition**: New features that change event sequences

**Do NOT update** when:
- Tests fail unexpectedly (investigate the cause first)
- Minor variations occur (adjust tolerances instead)
- Flaky timing issues appear (fix the issue, don't hide it)

## Updating Golden Recordings

```bash
# 1. Verify the new recording is correct manually
# 2. Run the test to see the differences
dotnet test --filter "Recording_MatchesGolden"

# 3. Review the diff carefully
# 4. If intentional, update the golden recording
cp new-recording.urf.jsonl test-data/golden/desktop/basic-movement.urf.jsonl

# 5. Re-run tests to confirm
dotnet test --filter "Recording_MatchesGolden"
```

## Best Practices

1. **Keep Scenarios Short**: 10-30 seconds max
2. **Be Deterministic**: Use fixed seeds, controlled inputs
3. **Document Scenarios**: Add comments explaining what each recording captures
4. **Version Control**: Commit golden recordings to git
5. **Review Changes**: Carefully review diffs when updating
6. **Multiple Scenarios**: Cover key user flows

## Troubleshooting

### Test Fails But Recording Looks Correct

Adjust comparison tolerances:
```csharp
var options = new ComparisonOptions
{
    EventCountTolerance = 0.2,  // Increase to 20%
    DurationTolerance = 0.25    // Increase to 25%
};
```

### Timing Variations Cause Failures

Use structural comparison:
```csharp
var options = new ComparisonOptions
{
    CompareStructureOnly = true  // Only compare event types/counts
};
```

### Golden Recording is Outdated

1. Capture new recording
2. Compare manually
3. Update if behavior is intentional
4. Document why in commit message

## Examples

See `GoldenRecordingTests.cs` for test examples.

## Questions?

See:
- `docs/URF-Automated-Testing-Design.md` - Overall design
- `docs/STEP3-COMPLETE.md` - Golden system implementation details
