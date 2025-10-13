using System.IO;
using System.Linq;
using Plate.CrossMilo.Contracts.Recorder.URF;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;
using Xunit;

namespace CrossMilo.Tests.URF;

/// <summary>
/// Tests for golden recording comparison and regression detection
/// </summary>
public class GoldenRecordingTests
{
    private const string TestDataPath = "../../../../../tests/test-data";

    [Fact]
    public void RecordingComparer_IdenticalRecordings_AreEquivalent()
    {
        // Arrange
        var recordingFile = FindAnyTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);
        var comparer = new RecordingComparer();

        // Act - compare recording to itself
        var result = comparer.Compare(events, events);

        // Assert
        Assert.True(result.IsEquivalent);
        Assert.Equal(1.0, result.SimilarityScore, 2);
        Assert.Empty(result.Differences);
    }

    [Fact]
    public void RecordingComparer_SimilarRecordings_DetectsDifferences()
    {
        // Arrange
        var recordingFile = FindAnyTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var original = URFReader.Load(recordingFile);

        // Create a modified version with different event count
        var modified = original.Take(original.Count - 1).ToList();

        var comparer = new RecordingComparer();

        // Act
        var result = comparer.Compare(modified, original);

        // Assert
        Assert.False(result.IsEquivalent); // Should detect the difference
        Assert.NotEmpty(result.Differences);
    }

    [Fact]
    public void RecordingComparer_WithinTolerance_IsEquivalent()
    {
        // Arrange
        var recordingFile = FindAnyTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var original = URFReader.Load(recordingFile);

        // Create a version with slightly different event count (within 10% tolerance)
        var outputEvents = original.GetEvents<OutputEvent>().ToList();
        if (outputEvents.Count < 10)
        {
            return; // Need enough events for test
        }

        var modified = original.Where(e => !(e is OutputEvent output) || outputEvents.IndexOf(output) % 20 != 0).ToList();

        var comparer = new RecordingComparer(new ComparisonOptions
        {
            EventCountTolerance = 0.1 // 10% tolerance
        });

        // Act
        var result = comparer.Compare(modified, original);

        // Assert - should be similar enough
        Assert.True(result.SimilarityScore > 0.8,
            $"Similarity should be > 80%, got {result.SimilarityScore:P1}");
    }

    [Fact]
    public void RecordingComparer_ComparesEventCounts()
    {
        // Arrange
        var recordingFile = FindAnyTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);
        var comparer = new RecordingComparer();

        // Act
        var result = comparer.Compare(events, events);

        // Assert
        Assert.NotEmpty(result.ActualEventCounts);
        Assert.NotEmpty(result.GoldenEventCounts);
        Assert.Equal(result.ActualEventCounts, result.GoldenEventCounts);
    }

    [Fact]
    public void RecordingComparer_ComparesDuration()
    {
        // Arrange
        var recordingFile = FindAnyTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);
        var comparer = new RecordingComparer();

        // Act
        var result = comparer.Compare(events, events);

        // Assert
        Assert.True(result.ActualDuration > 0);
        Assert.Equal(result.ActualDuration, result.GoldenDuration, 2);
    }

    [Fact]
    public void RecordingComparer_StructureOnly_IgnoresContent()
    {
        // Arrange
        var recordingFile = FindAnyTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var original = URFReader.Load(recordingFile);

        // Create a modified version with same structure but different content
        var modified = original.Select(e =>
        {
            if (e is OutputEvent output)
            {
                return new OutputEvent
                {
                    Timestamp = output.Timestamp,
                    Stream = output.Stream,
                    Sequence = output.Sequence,
                    Data = new { modified = true } // Different data
                };
            }
            return e;
        }).ToList();

        var comparer = new RecordingComparer(new ComparisonOptions
        {
            CompareStructureOnly = true
        });

        // Act
        var result = comparer.Compare(modified, original);

        // Assert - should still be equivalent structurally
        Assert.True(result.SimilarityScore >= 0.9);
    }

    [Fact]
    public void RecordingComparer_DetectsMissingEventTypes()
    {
        // Arrange
        var recordingFile = FindAnyTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var original = URFReader.Load(recordingFile);

        // Remove all events of one type
        var eventTypes = original.Select(e => e.Type).Distinct().ToList();
        if (eventTypes.Count < 2)
        {
            return; // Need multiple types
        }

        var typeToRemove = eventTypes.First(t => t != "meta");
        var modified = original.Where(e => e.Type != typeToRemove).ToList();

        var comparer = new RecordingComparer();

        // Act
        var result = comparer.Compare(modified, original);

        // Assert
        Assert.False(result.IsEquivalent);
        Assert.Contains(result.Differences, d => d.Contains("Missing event type"));
    }

    [Fact]
    public void RecordingComparer_Summary_ProvidesUsefulInfo()
    {
        // Arrange
        var recordingFile = FindAnyTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);
        var comparer = new RecordingComparer();

        // Act
        var result = comparer.Compare(events, events);

        // Assert
        Assert.NotNull(result.Summary);
        Assert.Contains("equivalent", result.Summary.ToLower());
    }

    [Fact]
    public void ComparisonOptions_DefaultTolerance_IsReasonable()
    {
        // Arrange
        var options = new ComparisonOptions();

        // Assert - verify default values are reasonable
        Assert.True(options.TimestampTolerance > 0);
        Assert.True(options.TimestampTolerance < 1.0);

        Assert.True(options.EventCountTolerance > 0);
        Assert.True(options.EventCountTolerance < 0.5);

        Assert.True(options.DurationTolerance > 0);
        Assert.True(options.DurationTolerance < 0.5);
    }

    [Fact]
    public void Golden_Recording_CanBeUsedForRegression()
    {
        // Arrange - try to find a golden recording
        var goldenPath = Path.Combine(TestDataPath, "golden", "desktop", "poc-reference.urf.jsonl");

        if (!File.Exists(goldenPath))
        {
            // If no golden recording yet, use any recording as both golden and actual
            goldenPath = FindAnyTestRecording();
            if (goldenPath == null)
            {
                return;
            }
        }

        var golden = URFReader.Load(goldenPath);
        var actual = golden; // For this test, use same recording

        var comparer = new RecordingComparer();

        // Act
        var result = comparer.Compare(actual, golden);

        // Assert - should be equivalent
        Assert.True(result.IsEquivalent,
            $"Golden comparison should pass. Differences: {string.Join(", ", result.Differences)}");
    }

    private string? FindAnyTestRecording()
    {
        // Try POC recording first
        var pocPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "../../../../../samples/urf-poc/bin/Debug/net8.0/recordings/urf"
        );

        if (Directory.Exists(pocPath))
        {
            var files = Directory.GetFiles(pocPath, "*.urf.jsonl");
            if (files.Length > 0)
            {
                return files.OrderByDescending(f => File.GetLastWriteTime(f)).First();
            }
        }

        // Try test-data directories
        var testDataDirs = new[]
        {
            Path.Combine(TestDataPath, "desktop"),
            Path.Combine(TestDataPath, "console"),
            Path.Combine(TestDataPath, "golden", "desktop"),
            Path.Combine(TestDataPath, "golden", "console")
        };

        foreach (var dir in testDataDirs)
        {
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, "*.urf.jsonl");
                if (files.Length > 0)
                {
                    return files.OrderByDescending(f => File.GetLastWriteTime(f)).First();
                }
            }
        }

        return null;
    }
}
