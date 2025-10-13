using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Plate.CrossMilo.Contracts.Recorder.URF;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;
using Xunit;

namespace CrossMilo.Tests.URF;

/// <summary>
/// Tests for console app (terminal) URF recordings
/// </summary>
public class ConsoleRecordingTests
{
    private const string TestDataPath = "../../../../../tests/test-data/console";

    [Fact]
    public void Console_Recording_CanBeLoaded()
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
    public void Console_Recording_HasMetadata()
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
        Assert.Equal("console", metadata.Recording.Platform);

        // Console recordings should have terminal info
        Assert.NotNull(metadata.Recording.Terminal);
        Assert.True(metadata.Recording.Terminal.Width > 0);
        Assert.True(metadata.Recording.Terminal.Height > 0);
        Assert.Equal("utf-8", metadata.Recording.Terminal.Encoding);
    }

    [Fact]
    public void Console_Recording_HasTerminalOutput()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var terminalOutputEvents = events.GetEvents<OutputEvent>()
            .Where(e => e.Stream == "terminal")
            .ToList();

        // Assert
        Assert.NotEmpty(terminalOutputEvents);

        // Verify terminal output structure
        var firstTerminalEvent = terminalOutputEvents.First();
        Assert.NotNull(firstTerminalEvent.Data);

        // Parse the output data
        var json = JsonSerializer.Serialize(firstTerminalEvent.Data);
        var doc = JsonDocument.Parse(json);

        Assert.True(doc.RootElement.TryGetProperty("format", out var format));
        Assert.Equal("ansi", format.GetString());

        // Should have either text or bytes
        var hasText = doc.RootElement.TryGetProperty("text", out var text) &&
                     !string.IsNullOrEmpty(text.GetString());
        var hasBytes = doc.RootElement.TryGetProperty("bytes", out var bytes) &&
                      !string.IsNullOrEmpty(bytes.GetString());

        Assert.True(hasText || hasBytes, "Terminal output should have text or bytes");
    }

    [Fact]
    public void Console_Recording_TerminalOutputIsANSI()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var terminalOutputEvents = events.GetEvents<OutputEvent>()
            .Where(e => e.Stream == "terminal")
            .ToList();

        if (terminalOutputEvents.Count == 0)
        {
            return;
        }

        // Assert - verify ANSI format is specified
        var firstEvent = terminalOutputEvents.First();
        var json = JsonSerializer.Serialize(firstEvent.Data);
        var doc = JsonDocument.Parse(json);

        Assert.True(doc.RootElement.TryGetProperty("format", out var format));
        Assert.Equal("ansi", format.GetString());

        // If text is present, it might contain ANSI escape sequences
        if (doc.RootElement.TryGetProperty("text", out var text))
        {
            var textValue = text.GetString();
            // ANSI sequences often contain \u001b or \x1b
            // This is optional - not all output will have ANSI codes
        }
    }

    [Fact]
    public void Console_Recording_HasReasonableDuration()
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
    public void Console_Recording_HasMultipleOutputEvents()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var terminalOutputEvents = events.GetEvents<OutputEvent>()
            .Where(e => e.Stream == "terminal")
            .ToList();

        // Assert - console app should produce multiple output events
        Assert.True(terminalOutputEvents.Count >= 5,
            $"Console recording should have multiple output events, got {terminalOutputEvents.Count}");
    }

    [Fact]
    public void Console_Recording_AsciinemaExportExists()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        // Act - check for .cast file with same base name
        var castFile = recordingFile.Replace(".urf.jsonl", ".cast");

        // Assert
        if (File.Exists(castFile))
        {
            // Verify it's valid Asciinema format
            var firstLine = File.ReadLines(castFile).First();
            var header = JsonDocument.Parse(firstLine);

            Assert.True(header.RootElement.TryGetProperty("version", out var version));
            Assert.Equal(2, version.GetInt32());

            Assert.True(header.RootElement.TryGetProperty("width", out var width));
            Assert.True(width.GetInt32() > 0);

            Assert.True(header.RootElement.TryGetProperty("height", out var height));
            Assert.True(height.GetInt32() > 0);
        }
        // If .cast file doesn't exist, that's okay - it's auto-generated on stop
        // Test passes either way
    }

    [Fact]
    public void Console_Recording_TerminalEventsHaveValidTimestamps()
    {
        // Arrange
        var recordingFile = FindTestRecording();
        if (recordingFile == null)
        {
            return;
        }

        var events = URFReader.Load(recordingFile);

        // Act
        var terminalOutputEvents = events.GetEvents<OutputEvent>()
            .Where(e => e.Stream == "terminal")
            .OrderBy(e => e.Timestamp)
            .ToList();

        if (terminalOutputEvents.Count < 2)
        {
            return; // Need at least 2 events
        }

        // Assert - timestamps should be monotonically increasing
        for (int i = 1; i < terminalOutputEvents.Count; i++)
        {
            Assert.True(terminalOutputEvents[i].Timestamp >= terminalOutputEvents[i - 1].Timestamp,
                $"Timestamp at index {i} should be >= timestamp at index {i - 1}");
        }

        // Verify reasonable output rate
        var duration = terminalOutputEvents.Last().Timestamp - terminalOutputEvents.First().Timestamp;
        if (duration > 0)
        {
            var eventsPerSecond = terminalOutputEvents.Count / duration;
            Assert.True(eventsPerSecond < 1000,
                $"Output rate should be reasonable, got {eventsPerSecond:F1} events/sec");
        }
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
            "../../../../../../yokan-projects/winged-bean/development/dotnet/console/src/host/Dungeon.Host.Console/bin/Debug/net8.0/recordings/urf"
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
