using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Plate.CrossMilo.Contracts.Recorder.URF;

/// <summary>
/// Metadata for a recording session
/// </summary>
public class RecordingMetadata
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("started")]
    public DateTimeOffset Started { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("generator")]
    public string Generator { get; set; } = "URF Recorder";

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = "unknown";

    [JsonPropertyName("environment")]
    public Dictionary<string, string> Environment { get; set; } = new();

    [JsonPropertyName("application")]
    public ApplicationInfo Application { get; set; } = new();

    [JsonPropertyName("terminal")]
    public TerminalInfo? Terminal { get; set; }

    [JsonPropertyName("streams")]
    public List<string> Streams { get; set; } = new();
}

public class ApplicationInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";

    [JsonPropertyName("commit")]
    public string? Commit { get; set; }
}

public class TerminalInfo
{
    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("colorDepth")]
    public int ColorDepth { get; set; } = 256;

    [JsonPropertyName("encoding")]
    public string Encoding { get; set; } = "utf-8";
}
