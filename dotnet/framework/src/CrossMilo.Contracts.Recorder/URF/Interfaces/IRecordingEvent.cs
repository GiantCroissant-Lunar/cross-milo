using System.Text.Json.Serialization;

namespace Plate.CrossMilo.Contracts.Recorder.URF;

/// <summary>
/// Base interface for all Universal Recording Format events
/// </summary>
public interface IRecordingEvent
{
    /// <summary>Timestamp in seconds since recording start (high precision)</summary>
    [JsonPropertyName("t")]
    double Timestamp { get; set; }

    /// <summary>Event type discriminator (meta, input, output, state, metric, annotation, marker)</summary>
    [JsonPropertyName("type")]
    string Type { get; }

    /// <summary>Stream identifier (terminal, cells, gameplay, keyboard, mouse, performance, etc.)</summary>
    [JsonPropertyName("stream")]
    string? Stream { get; set; }

    /// <summary>Optional sequence number for ordering</summary>
    [JsonPropertyName("seq")]
    int? Sequence { get; set; }
}
