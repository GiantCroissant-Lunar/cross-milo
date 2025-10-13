using System.Text.Json.Serialization;

namespace Plate.CrossMilo.Contracts.Recorder.URF.Events;

/// <summary>
/// Recording metadata event (first event in recording)
/// </summary>
public class MetadataEvent : IRecordingEvent
{
    [JsonPropertyName("t")]
    public double Timestamp { get; set; } = 0;

    [JsonPropertyName("type")]
    public string Type => "meta";

    [JsonPropertyName("stream")]
    public string? Stream { get; set; }

    [JsonPropertyName("seq")]
    public int? Sequence { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";

    [JsonPropertyName("recording")]
    public RecordingMetadata Recording { get; set; } = new();
}
