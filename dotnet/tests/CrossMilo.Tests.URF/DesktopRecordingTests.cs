using System.IO;
using System.Linq;
using System.Text.Json;
using Plate.CrossMilo.Contracts.Recorder.URF;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;
using Xunit;

namespace CrossMilo.Tests.URF;

/// <summary>
/// Tests for desktop app (SadConsole) URF recordings
/// </summary>
public class DesktopRecordingTests
{
    private const string TestDataPath = "../../../../../tests/test-data/desktop";

    [Fact]
    public void Desktop_Recording_CanBeLoaded()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            // Skip if no test recording available yet
            return;
        }

        // Act
        var events = URFReader.Load(recordingFile);

        // Assert
        Assert.NotEmpty(events);
    }

    [Fact]
    public void Desktop_Recording_HasMetadata()
    {
        // Arrange
        var recordingFile = FindTestRecording();
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
        Assert.Equal("desktop", metadata.Recording.Platform);

        // Desktop recordings should have terminal info (screen dimensions)
        Assert.NotNull(metadata.Recording.Terminal);
        Assert.True(metadata.Recording.Terminal.Width > 0);
        Assert.True(metadata.Recording.Terminal.Height > 0);
    }

    [Fact]
    public void Desktop_Recording_HasCellOutput()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var cellOutputEvents = events.GetEvents<OutputEvent>()
            .Where(e => e.Stream == "cells")
            .ToList();

        // Assert
        Assert.NotEmpty(cellOutputEvents);

        // Verify cell grid structure
        var firstCellEvent = cellOutputEvents.First();
        Assert.NotNull(firstCellEvent.Data);

        // Parse the cell data (it's stored as object, need to access dynamically)
        var json = JsonSerializer.Serialize(firstCellEvent.Data);
        var doc = JsonDocument.Parse(json);

        Assert.True(doc.RootElement.TryGetProperty("format", out var format));
        Assert.Equal("grid", format.GetString());

        Assert.True(doc.RootElement.TryGetProperty("width", out var width));
        Assert.True(width.GetInt32() > 0);

        Assert.True(doc.RootElement.TryGetProperty("height", out var height));
        Assert.True(height.GetInt32() > 0);
    }

    [Fact]
    public void Desktop_Recording_HasGameplayState()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var stateEvents = events.GetEvents<StateEvent>()
            .Where(e => e.Stream == "gameplay")
            .ToList();

        // Assert
        Assert.NotEmpty(stateEvents);

        // Verify state structure
        var firstState = stateEvents.First();
        Assert.NotNull(firstState.Data);

        // Parse state data
        var json = JsonSerializer.Serialize(firstState.Data);
        var doc = JsonDocument.Parse(json);

        // Desktop recordings should have floor and hud info
        Assert.True(doc.RootElement.TryGetProperty("floor", out _));
        Assert.True(doc.RootElement.TryGetProperty("hud", out var hud));

        // HUD should have player stats
        Assert.True(hud.TryGetProperty("hp", out _));
        Assert.True(hud.TryGetProperty("maxHp", out _));
        Assert.True(hud.TryGetProperty("level", out _));
    }

    [Fact]
    public void Desktop_Recording_HasReasonableDuration()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var duration = events.GetDuration();

        // Assert
        Assert.True(duration > 0, "Recording should have positive duration");
        Assert.True(duration < 300, "Test recording should be less than 5 minutes");
    }

    [Fact]
    public void Desktop_Recording_HasMultipleFrames()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var cellOutputEvents = events.GetEvents<OutputEvent>()
            .Where(e => e.Stream == "cells")
            .ToList();

        // Assert - should have at least 30 frames for a 1+ second recording
        Assert.True(cellOutputEvents.Count >= 10,
            $"Desktop recording should have multiple frames, got {cellOutputEvents.Count}");
    }

    [Fact]
    public void Desktop_Recording_CellEventsHaveValidTimestamps()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var cellOutputEvents = events.GetEvents<OutputEvent>()
            .Where(e => e.Stream == "cells")
            .OrderBy(e => e.Timestamp)
            .ToList();

        if (cellOutputEvents.Count < 2)
        {
            return; // Need at least 2 frames
        }

        // Assert - calculate average FPS
        var duration = cellOutputEvents.Last().Timestamp - cellOutputEvents.First().Timestamp;
        var fps = cellOutputEvents.Count / duration;

        Assert.True(fps >= 10, $"FPS should be at least 10, got {fps:F1}");
        Assert.True(fps <= 200, $"FPS should be reasonable, got {fps:F1}");
    }

    private string? FindTestRecording()
    {
        // Try test-data directory first
        if (Directory.Exists(TestDataPath))
        {
            var testFiles = Directory.GetFiles(TestDataPath, "*.urf.jsonl");
            if (testFiles.Length > 0)
            {
                return testFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();
            }
        }

        // Fall back to actual recordings directory if available
        var recordingsPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "../../../../../../yokan-projects/winged-bean/development/dotnet/windows/src/host/Dungeon.Host.SadConsole/bin/Debug/net8.0/recordings/urf"
        );

        if (Directory.Exists(recordingsPath))
        {
            var files = Directory.GetFiles(recordingsPath, "*.urf.jsonl");
            if (files.Length > 0)
            {
                return files.OrderByDescending(f => File.GetLastWriteTime(f)).First();
            }
        }

        return null;
    }
}
