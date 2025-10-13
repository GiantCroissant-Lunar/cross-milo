using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Plate.CrossMilo.Contracts.Recorder.URF;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;
using Plate.CrossMilo.Contracts.Recorder.URF.Exporters;

Console.WriteLine("=== Universal Recording Format - Proof of Concept ===\n");

// Create recorder
var recorder = new UniversalRecorder();
var sessionId = "poc-demo";

// Start recording
Console.WriteLine("[1] Starting recording session...");
var metadata = new RecordingMetadata
{
    Id = sessionId,
    Started = DateTimeOffset.UtcNow,
    Generator = "URF POC v1.0",
    Platform = "console",
    Application = new ApplicationInfo
    {
        Name = "URF Proof of Concept",
        Version = "1.0.0"
    },
    Terminal = new TerminalInfo
    {
        Width = 80,
        Height = 24
    },
    Streams = new() { "terminal", "state", "metric" }
};

await recorder.StartRecordingAsync(sessionId, metadata);
Console.WriteLine("    ✓ Recording started\n");

// Simulate some events
Console.WriteLine("[2] Recording events...");
var startTime = DateTimeOffset.UtcNow;

// Terminal output event
await recorder.RecordEventAsync(sessionId, new OutputEvent
{
    Timestamp = 0.1,
    Stream = "terminal",
    Data = new TerminalOutputData
    {
        Format = "ansi",
        Text = "Hello, URF!\n"
    }
});
Console.WriteLine("    ✓ Recorded terminal output");

await Task.Delay(100);

// Terminal output event 2
await recorder.RecordEventAsync(sessionId, new OutputEvent
{
    Timestamp = 0.2,
    Stream = "terminal",
    Data = new TerminalOutputData
    {
        Format = "ansi",
        Text = "Recording gameplay events...\n"
    }
});
Console.WriteLine("    ✓ Recorded terminal output 2");

// State event
await recorder.RecordEventAsync(sessionId, new StateEvent
{
    Timestamp = 0.3,
    Stream = "gameplay",
    Data = new
    {
        player = new { hp = 100, level = 1, x = 10, y = 5 },
        floor = 1
    }
});
Console.WriteLine("    ✓ Recorded game state");

await Task.Delay(100);

// Cell grid output (desktop-style)
await recorder.RecordEventAsync(sessionId, new OutputEvent
{
    Timestamp = 0.4,
    Stream = "cells",
    Data = new CellGridOutputData
    {
        Width = 80,
        Height = 24,
        Cells = new()
        {
            new CellData { X = 10, Y = 5, Char = "@", Foreground = "yellow" },
            new CellData { X = 11, Y = 5, Char = ".", Foreground = "white" }
        }
    }
});
Console.WriteLine("    ✓ Recorded cell grid output");

// Input event
await recorder.RecordEventAsync(sessionId, new InputEvent
{
    Timestamp = 0.5,
    Stream = "keyboard",
    Data = new KeyboardInputData
    {
        Key = "ArrowUp",
        Pressed = true
    }
});
Console.WriteLine("    ✓ Recorded input event\n");

// Stop recording
Console.WriteLine("[3] Stopping recording...");
var urfPath = await recorder.StopRecordingAsync(sessionId);
Console.WriteLine($"    ✓ Recording saved: {urfPath}\n");

// Export to Asciinema
Console.WriteLine("[4] Exporting to Asciinema format...");
var exporter = new AsciinemaExporter();
var castPath = Path.Combine(Path.GetDirectoryName(urfPath)!, "poc-demo.cast");
await exporter.ExportAsync(urfPath, castPath);
Console.WriteLine($"    ✓ Asciinema export saved: {castPath}\n");

// Display file contents
Console.WriteLine("[5] Recording contents (first 10 lines):");
Console.WriteLine("─────────────────────────────────────────");
var lines = await File.ReadAllLinesAsync(urfPath);
foreach (var line in lines.Take(10))
{
    Console.WriteLine(line.Length > 120 ? line.Substring(0, 120) + "..." : line);
}
if (lines.Length > 10)
{
    Console.WriteLine($"... ({lines.Length - 10} more lines)");
}
Console.WriteLine("─────────────────────────────────────────\n");

Console.WriteLine("[6] Asciinema export contents:");
Console.WriteLine("─────────────────────────────────────────");
var castLines = await File.ReadAllLinesAsync(castPath);
foreach (var line in castLines)
{
    Console.WriteLine(line);
}
Console.WriteLine("─────────────────────────────────────────\n");

Console.WriteLine("✅ POC Complete!");
Console.WriteLine($"\n📁 URF Recording: {urfPath}");
Console.WriteLine($"📁 Asciinema Export: {castPath}");
Console.WriteLine($"\n💡 Try: asciinema play {castPath}");
