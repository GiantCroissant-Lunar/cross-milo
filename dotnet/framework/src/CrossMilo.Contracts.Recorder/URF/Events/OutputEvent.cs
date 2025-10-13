using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Plate.CrossMilo.Contracts.Recorder.URF.Events;

/// <summary>
/// Output event (terminal bytes or cell grid)
/// </summary>
public class OutputEvent : IRecordingEvent
{
    [JsonPropertyName("t")]
    public double Timestamp { get; set; }

    [JsonPropertyName("type")]
    public string Type => "output";

    [JsonPropertyName("stream")]
    public string? Stream { get; set; }

    [JsonPropertyName("seq")]
    public int? Sequence { get; set; }

    [JsonPropertyName("data")]
    public object Data { get; set; } = new object();
}

/// <summary>
/// Terminal output data (byte stream)
/// </summary>
public class TerminalOutputData
{
    [JsonPropertyName("format")]
    public string Format { get; set; } = "ansi";

    [JsonPropertyName("bytes")]
    public string? Bytes { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

/// <summary>
/// Cell grid output data (for SadConsole, etc.)
/// </summary>
public class CellGridOutputData
{
    [JsonPropertyName("format")]
    public string Format { get; set; } = "grid";

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("cells")]
    public List<CellData> Cells { get; set; } = new();
}

public class CellData
{
    [JsonPropertyName("x")]
    public int X { get; set; }

    [JsonPropertyName("y")]
    public int Y { get; set; }

    [JsonPropertyName("ch")]
    public string Char { get; set; } = " ";

    [JsonPropertyName("fg")]
    public string? Foreground { get; set; }

    [JsonPropertyName("bg")]
    public string? Background { get; set; }
}
