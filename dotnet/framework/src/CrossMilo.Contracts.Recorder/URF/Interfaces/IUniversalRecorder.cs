using System.Threading;
using System.Threading.Tasks;

namespace Plate.CrossMilo.Contracts.Recorder.URF;

/// <summary>
/// Universal recording service - records events in URF format
/// </summary>
public interface IUniversalRecorder
{
    /// <summary>
    /// Start a new recording session
    /// </summary>
    Task StartRecordingAsync(string sessionId, RecordingMetadata metadata, CancellationToken ct = default);

    /// <summary>
    /// Record an event to the active session
    /// </summary>
    Task RecordEventAsync(string sessionId, IRecordingEvent evt, CancellationToken ct = default);

    /// <summary>
    /// Stop recording and finalize the session
    /// </summary>
    /// <returns>Path to the URF recording file</returns>
    Task<string> StopRecordingAsync(string sessionId, CancellationToken ct = default);

    /// <summary>
    /// Check if a session is currently recording
    /// </summary>
    bool IsRecording(string sessionId);
}
