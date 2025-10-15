namespace Plate.CrossMilo.Contracts.Recorder.Video;

/// <summary>
/// Interface for exporting recordings to video format.
/// Implementations can export from various sources (URF, raw frames, etc.) to video files.
/// </summary>
public interface IVideoExporter
{
    /// <summary>
    /// Exports a recording to video format.
    /// </summary>
    /// <param name="sourcePath">Path to the source recording file</param>
    /// <param name="outputPath">Path to the output video file</param>
    /// <param name="options">Export options (codec, quality, etc.)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task ExportAsync(
        string sourcePath,
        string outputPath,
        VideoExportOptions? options = null,
        CancellationToken ct = default);

    /// <summary>
    /// Exports a recording to video format with progress reporting.
    /// </summary>
    /// <param name="sourcePath">Path to the source recording file</param>
    /// <param name="outputPath">Path to the output video file</param>
    /// <param name="options">Export options (codec, quality, etc.)</param>
    /// <param name="progress">Progress reporter</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task ExportAsync(
        string sourcePath,
        string outputPath,
        VideoExportOptions? options,
        IProgress<VideoExportProgress>? progress,
        CancellationToken ct = default);
}

/// <summary>
/// Options for video export.
/// </summary>
public class VideoExportOptions
{
    /// <summary>
    /// Video codec to use (e.g., "h264", "vp9", "gif").
    /// </summary>
    public string Codec { get; set; } = "h264";

    /// <summary>
    /// Encoding preset (e.g., "ultrafast", "fast", "medium", "slow").
    /// </summary>
    public string Preset { get; set; } = "medium";

    /// <summary>
    /// Quality level (codec-specific, e.g., CRF for H.264).
    /// </summary>
    public int Quality { get; set; } = 23;

    /// <summary>
    /// Target frame rate (0 = use source frame rate).
    /// </summary>
    public int FrameRate { get; set; } = 0;

    /// <summary>
    /// Speed multiplier (1.0 = normal speed, 2.0 = 2x speed, 0.5 = half speed).
    /// </summary>
    public double SpeedMultiplier { get; set; } = 1.0;

    /// <summary>
    /// Include timestamp overlay in video.
    /// </summary>
    public bool IncludeTimestamp { get; set; } = false;

    /// <summary>
    /// Include HUD/UI overlay in video.
    /// </summary>
    public bool IncludeHUD { get; set; } = true;

    /// <summary>
    /// Filter specific streams (e.g., "cells", "gameplay").
    /// Null = include all streams.
    /// </summary>
    public string? StreamFilter { get; set; }
}

/// <summary>
/// Progress information for video export.
/// </summary>
public class VideoExportProgress
{
    /// <summary>
    /// Current frame being processed.
    /// </summary>
    public long CurrentFrame { get; set; }

    /// <summary>
    /// Total number of frames to process.
    /// </summary>
    public long TotalFrames { get; set; }

    /// <summary>
    /// Percentage complete (0-100).
    /// </summary>
    public double PercentComplete => TotalFrames > 0 
        ? (double)CurrentFrame / TotalFrames * 100.0 
        : 0.0;

    /// <summary>
    /// Current encoding speed (frames per second).
    /// </summary>
    public double EncodingFPS { get; set; }

    /// <summary>
    /// Estimated time remaining.
    /// </summary>
    public TimeSpan? EstimatedTimeRemaining { get; set; }

    /// <summary>
    /// Current status message.
    /// </summary>
    public string? StatusMessage { get; set; }
}
