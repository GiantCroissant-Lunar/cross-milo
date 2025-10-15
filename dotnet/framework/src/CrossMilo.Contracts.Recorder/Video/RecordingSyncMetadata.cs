using System;
using System.Collections.Generic;

namespace Plate.CrossMilo.Contracts.Recorder.Video;

/// <summary>
/// Synchronization metadata for hybrid URF+Video recordings.
/// Links video and URF files on a common timeline.
/// </summary>
public class RecordingSyncMetadata
{
    /// <summary>
    /// Metadata format version.
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Unique session identifier.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    /// Recording start time (UTC).
    /// </summary>
    public DateTimeOffset StartTime { get; set; }

    /// <summary>
    /// Video file name (relative to sync file).
    /// </summary>
    public string? VideoFile { get; set; }

    /// <summary>
    /// URF file name (relative to sync file).
    /// </summary>
    public string URFFile { get; set; } = string.Empty;

    /// <summary>
    /// Video frame rate (if video recording is enabled).
    /// </summary>
    public int? FPS { get; set; }

    /// <summary>
    /// Video width in pixels (if video recording is enabled).
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Video height in pixels (if video recording is enabled).
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Total recording duration in milliseconds.
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Whether video recording was enabled for this session.
    /// </summary>
    public bool HasVideo => !string.IsNullOrEmpty(VideoFile);
}
