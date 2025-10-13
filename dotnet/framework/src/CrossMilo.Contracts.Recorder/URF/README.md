# Universal Recording Format (URF)

A general-purpose recording format for capturing application events (input, output, state, metrics) with export capabilities to specialized formats like Asciinema.

## Quick Start

```csharp
using Plate.CrossMilo.Contracts.Recorder.URF;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;
using Plate.CrossMilo.Contracts.Recorder.URF.Exporters;

// Create recorder
var recorder = new UniversalRecorder();

// Start recording
var metadata = new RecordingMetadata
{
    Id = "my-session",
    Platform = "console",
    Application = new ApplicationInfo { Name = "My App", Version = "1.0.0" },
    Terminal = new TerminalInfo { Width = 80, Height = 24 }
};
await recorder.StartRecordingAsync("my-session", metadata);

// Record events
await recorder.RecordEventAsync("my-session", new OutputEvent
{
    Timestamp = 0.1,
    Stream = "terminal",
    Data = new TerminalOutputData { Text = "Hello, World!\n" }
});

// Stop recording
var urfPath = await recorder.StopRecordingAsync("my-session");
// Result: recordings/urf/session-my-session-{timestamp}.urf.jsonl

// Export to Asciinema
var exporter = new AsciinemaExporter();
await exporter.ExportAsync(urfPath, "output.cast");
// Result: output.cast (Asciinema v2 format)
```

## Event Types

### MetadataEvent
First event in every recording, contains session information.

### OutputEvent
Terminal or cell grid output.

**Terminal Output**:
```csharp
new OutputEvent
{
    Stream = "terminal",
    Data = new TerminalOutputData { Text = "output text" }
}
```

**Cell Grid Output**:
```csharp
new OutputEvent
{
    Stream = "cells",
    Data = new CellGridOutputData 
    {
        Width = 80,
        Height = 24,
        Cells = new List<CellData> 
        {
            new() { X = 10, Y = 5, Char = "@", Foreground = "yellow" }
        }
    }
}
```

### StateEvent
Application or game state snapshots.

```csharp
new StateEvent
{
    Stream = "gameplay",
    Data = new { player = new { hp = 100, level = 1 }, floor = 1 }
}
```

### InputEvent
User input events.

```csharp
new InputEvent
{
    Stream = "keyboard",
    Data = new KeyboardInputData { Key = "ArrowUp", Pressed = true }
}
```

## Exporters

### AsciinemaExporter
Exports URF recordings to Asciinema v2 format (.cast files).

```csharp
var exporter = new AsciinemaExporter();
await exporter.ExportAsync("recording.urf.jsonl", "output.cast");
```

Only exports terminal output events; filters out state, input, and cell events.

## File Format

**Extension**: `.urf.jsonl`  
**Format**: JSONL (JSON Lines) - one JSON object per line  
**Encoding**: UTF-8

Example:
```jsonl
{"t":0,"type":"meta","version":"1.0","recording":{...}}
{"t":0.1,"type":"output","stream":"terminal","data":{...}}
{"t":0.2,"type":"state","stream":"gameplay","data":{...}}
```

## Integration Examples

### Console App
```csharp
public class ConsoleURFRecorder
{
    private readonly IUniversalRecorder _recorder;
    private DateTimeOffset _startTime;
    
    public async Task StartAsync(string sessionId)
    {
        _startTime = DateTimeOffset.UtcNow;
        await _recorder.StartRecordingAsync(sessionId, new RecordingMetadata
        {
            Platform = "console",
            Terminal = new TerminalInfo { Width = 80, Height = 24 }
        });
    }
    
    public async Task RecordOutputAsync(string sessionId, byte[] output)
    {
        var elapsed = (DateTimeOffset.UtcNow - _startTime).TotalSeconds;
        await _recorder.RecordEventAsync(sessionId, new OutputEvent
        {
            Timestamp = elapsed,
            Stream = "terminal",
            Data = new TerminalOutputData 
            {
                Bytes = Convert.ToBase64String(output),
                Text = Encoding.UTF8.GetString(output)
            }
        });
    }
}
```

### Desktop App (SadConsole)
```csharp
public class DesktopURFRecorder
{
    private readonly IUniversalRecorder _recorder;
    private DateTimeOffset _startTime;
    
    public async Task RecordCellsAsync(string sessionId, IEnumerable<Cell> cells)
    {
        var elapsed = (DateTimeOffset.UtcNow - _startTime).TotalSeconds;
        await _recorder.RecordEventAsync(sessionId, new OutputEvent
        {
            Timestamp = elapsed,
            Stream = "cells",
            Data = new CellGridOutputData
            {
                Width = 120,
                Height = 40,
                Cells = cells.Select(c => new CellData
                {
                    X = c.X,
                    Y = c.Y,
                    Char = c.Glyph.ToString(),
                    Foreground = c.Foreground.ToString()
                }).ToList()
            }
        });
    }
    
    public async Task RecordStateAsync(string sessionId, object gameState)
    {
        var elapsed = (DateTimeOffset.UtcNow - _startTime).TotalSeconds;
        await _recorder.RecordEventAsync(sessionId, new StateEvent
        {
            Timestamp = elapsed,
            Stream = "gameplay",
            Data = gameState
        });
    }
}
```

## Querying Recordings

### With jq
```bash
# Get all state events
cat recording.urf.jsonl | jq 'select(.type == "state")'

# Get player HP at each state event
cat recording.urf.jsonl | jq 'select(.type == "state") | .data.player.hp'

# Get events in first 10 seconds
cat recording.urf.jsonl | jq 'select(.t < 10.0)'
```

### With C#
```csharp
var lines = await File.ReadAllLinesAsync("recording.urf.jsonl");
var events = lines.Select(JsonSerializer.Deserialize<IRecordingEvent>);

// Find when player reached level 2
var level2Event = events
    .OfType<StateEvent>()
    .FirstOrDefault(e => ((dynamic)e.Data).player.level == 2);
```

## Documentation

- **Design**: `/docs/Universal-Recording-Format-Design.md`
- **POC Results**: `/docs/URF-POC-Results.md`
- **Strategy**: `/docs/Runtime-Testing-Recording-Strategy.md`
- **POC Sample**: `/samples/urf-poc/`

## Version

**Current**: 1.0  
**Status**: Proof of Concept Complete
