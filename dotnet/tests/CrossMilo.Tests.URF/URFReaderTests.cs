using System.IO;
using System.Linq;
using Plate.CrossMilo.Contracts.Recorder.URF;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;
using Xunit;

namespace CrossMilo.Tests.URF;

public class URFReaderTests
{
    private readonly string _sampleRecordingPath;

    public URFReaderTests()
    {
        // Use the POC recording from samples
        _sampleRecordingPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "../../../../../samples/urf-poc/bin/Debug/net8.0/recordings/urf"
        );
    }

    [Fact]
    public void Load_ValidRecording_ReturnsEvents()
    {
        // Arrange - find the most recent .urf.jsonl file
        var recordingFile = Directory.GetFiles(_sampleRecordingPath, "*.urf.jsonl")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .FirstOrDefault();

        if (recordingFile == null)
        {
            // Skip test if no recording exists yet
            return;
        }

        // Act
        var events = URFReader.Load(recordingFile);

        // Assert
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e.Type == "meta");
    }

    [Fact]
    public void GetMetadata_ValidRecording_ReturnsMetadata()
    {
        // Arrange
        var recordingFile = Directory.GetFiles(_sampleRecordingPath, "*.urf.jsonl")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .FirstOrDefault();

        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var metadata = events.GetMetadata();

        // Assert
        Assert.NotNull(metadata);
        Assert.Equal("meta", metadata.Type);
        Assert.NotNull(metadata.Recording);
        Assert.False(string.IsNullOrEmpty(metadata.Recording.Platform));
    }

    [Fact]
    public void GetEvents_ByType_ReturnsFilteredEvents()
    {
        // Arrange
        var recordingFile = Directory.GetFiles(_sampleRecordingPath, "*.urf.jsonl")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .FirstOrDefault();

        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var outputEvents = events.GetEvents<OutputEvent>().ToList();
        var stateEvents = events.GetEvents<StateEvent>().ToList();

        // Assert
        Assert.NotEmpty(outputEvents);
        Assert.All(outputEvents, e => Assert.Equal("output", e.Type));

        Assert.NotEmpty(stateEvents);
        Assert.All(stateEvents, e => Assert.Equal("state", e.Type));
    }

    [Fact]
    public void GetDuration_ValidRecording_ReturnsPositiveDuration()
    {
        // Arrange
        var recordingFile = Directory.GetFiles(_sampleRecordingPath, "*.urf.jsonl")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .FirstOrDefault();

        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var duration = events.GetDuration();

        // Assert
        Assert.True(duration > 0);
    }

    [Fact]
    public void GetEventCounts_ValidRecording_ReturnsAccurateCounts()
    {
        // Arrange
        var recordingFile = Directory.GetFiles(_sampleRecordingPath, "*.urf.jsonl")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .FirstOrDefault();

        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var counts = events.GetEventCounts();

        // Assert
        Assert.True(counts.ContainsKey("meta"));
        Assert.Equal(1, counts["meta"]);
        Assert.True(counts.ContainsKey("output"));
        Assert.True(counts.ContainsKey("state"));
    }

    [Fact]
    public void Events_HasMonotonicTimestamps()
    {
        // Arrange
        var recordingFile = Directory.GetFiles(_sampleRecordingPath, "*.urf.jsonl")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .FirstOrDefault();

        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var timestamps = events.Select(e => e.Timestamp).ToList();

        // Assert - timestamps should be monotonically increasing
        for (int i = 1; i < timestamps.Count; i++)
        {
            Assert.True(timestamps[i] >= timestamps[i - 1],
                $"Timestamp at index {i} ({timestamps[i]}) should be >= timestamp at index {i - 1} ({timestamps[i - 1]})");
        }
    }

    [Fact]
    public void GetEventsInRange_ReturnsOnlyEventsInRange()
    {
        // Arrange
        var recordingFile = Directory.GetFiles(_sampleRecordingPath, "*.urf.jsonl")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .FirstOrDefault();

        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);
        var startTime = 0.1;
        var endTime = 0.3;

        // Act
        var rangeEvents = events.GetEventsInRange(startTime, endTime).ToList();

        // Assert
        Assert.NotEmpty(rangeEvents);
        Assert.All(rangeEvents, e =>
        {
            Assert.True(e.Timestamp >= startTime);
            Assert.True(e.Timestamp <= endTime);
        });
    }
}
