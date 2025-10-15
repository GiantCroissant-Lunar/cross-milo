using System;
using System.Threading;
using System.Threading.Tasks;

namespace Plate.CrossMilo.Contracts.Recorder.Video;

/// <summary>
/// Interface for video encoding services.
/// Implementations provide video encoding capabilities (e.g., FFmpeg, SharpAvi, etc.).
/// </summary>
public interface IVideoEncoder : IDisposable
{
    /// <summary>
    /// Gets whether the encoder is currently recording.
    /// </summary>
    bool IsRecording { get; }

    /// <summary>
    /// Gets the number of frames encoded so far.
    /// </summary>
    long FrameCount { get; }

    /// <summary>
    /// Gets the duration of the current recording.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// Starts recording to the specified output file.
    /// </summary>
    /// <param name="outputPath">Path to the output video file</param>
    /// <param name="width">Video width in pixels</param>
    /// <param name="height">Video height in pixels</param>
    /// <param name="fps">Frames per second</param>
    /// <exception cref="InvalidOperationException">If already recording</exception>
    /// <exception cref="ArgumentException">If parameters are invalid</exception>
    void StartRecording(string outputPath, int width, int height, int fps);

    /// <summary>
    /// Encodes a single frame from RGBA data.
    /// </summary>
    /// <param name="rgbaData">Frame data in RGBA format (4 bytes per pixel)</param>
    /// <param name="width">Frame width in pixels</param>
    /// <param name="height">Frame height in pixels</param>
    /// <param name="timestampMs">Timestamp in milliseconds from start of recording</param>
    /// <exception cref="InvalidOperationException">If not currently recording</exception>
    void EncodeFrame(byte[] rgbaData, int width, int height, long timestampMs);

    /// <summary>
    /// Encodes a single frame from RGBA data (zero-copy version).
    /// </summary>
    /// <param name="rgbaData">Frame data in RGBA format (4 bytes per pixel)</param>
    /// <param name="width">Frame width in pixels</param>
    /// <param name="height">Frame height in pixels</param>
    /// <param name="timestampMs">Timestamp in milliseconds from start of recording</param>
    /// <exception cref="InvalidOperationException">If not currently recording</exception>
    void EncodeFrame(ReadOnlySpan<byte> rgbaData, int width, int height, long timestampMs);

    /// <summary>
    /// Stops recording and finalizes the output file.
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    /// <exception cref="InvalidOperationException">If not currently recording</exception>
    Task StopRecordingAsync(CancellationToken ct = default);
}
