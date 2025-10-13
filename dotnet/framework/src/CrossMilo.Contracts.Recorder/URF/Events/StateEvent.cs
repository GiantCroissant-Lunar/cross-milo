using System.Text.Json.Serialization;

namespace Plate.CrossMilo.Contracts.Recorder.URF.Events;

/// <summary>
/// State snapshot event (game state, application state)
/// </summary>
public class StateEvent : IRecordingEvent
{
    [JsonPropertyName("t")]
    public double Timestamp { get; set; }

    [JsonPropertyName("type")]
    public string Type => "state";

    [JsonPropertyName("stream")]
    public string? Stream { get; set; }

    [JsonPropertyName("seq")]
    public int? Sequence { get; set; }

    [JsonPropertyName("data")]
    public object Data { get; set; } = new object();
}
