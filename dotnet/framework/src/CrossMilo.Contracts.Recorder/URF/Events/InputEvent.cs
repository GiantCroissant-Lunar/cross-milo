using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Plate.CrossMilo.Contracts.Recorder.URF.Events;

/// <summary>
/// Input event (keyboard, mouse)
/// </summary>
public class InputEvent : IRecordingEvent
{
    [JsonPropertyName("t")]
    public double Timestamp { get; set; }

    [JsonPropertyName("type")]
    public string Type => "input";

    [JsonPropertyName("stream")]
    public string? Stream { get; set; }

    [JsonPropertyName("seq")]
    public int? Sequence { get; set; }

    [JsonPropertyName("data")]
    public object Data { get; set; } = new object();
}

public class KeyboardInputData
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("modifiers")]
    public List<string> Modifiers { get; set; } = new();

    [JsonPropertyName("pressed")]
    public bool Pressed { get; set; }
}
