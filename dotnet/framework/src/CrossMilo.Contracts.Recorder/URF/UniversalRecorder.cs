using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;

namespace Plate.CrossMilo.Contracts.Recorder.URF;

/// <summary>
/// Universal recorder implementation - records events to JSONL format
/// </summary>
public class UniversalRecorder : IUniversalRecorder, IDisposable
{
    private readonly string _recordingsDirectory;
    private readonly Dictionary<string, RecordingSession> _sessions = new();
    private readonly object _lock = new();

    public UniversalRecorder(string? recordingsDirectory = null)
    {
        _recordingsDirectory = recordingsDirectory ?? Path.Combine(AppContext.BaseDirectory, "recordings", "urf");
        Directory.CreateDirectory(_recordingsDirectory);
    }

    public Task StartRecordingAsync(string sessionId, RecordingMetadata metadata, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (_sessions.ContainsKey(sessionId))
            {
                throw new InvalidOperationException($"Session {sessionId} is already recording");
            }

            var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMdd-HHmmss");
            var fileName = $"session-{sessionId}-{timestamp}.urf.jsonl";
            var filePath = Path.Combine(_recordingsDirectory, fileName);

            var session = new RecordingSession
            {
                SessionId = sessionId,
                FilePath = filePath,
                StartTime = DateTimeOffset.UtcNow,
                Writer = new StreamWriter(File.OpenWrite(filePath)) { AutoFlush = true }
            };

            // Write metadata event
            var metaEvent = new MetadataEvent
            {
                Timestamp = 0,
                Recording = metadata
            };

            var json = JsonSerializer.Serialize(metaEvent, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            session.Writer.WriteLine(json);

            _sessions[sessionId] = session;
        }

        return Task.CompletedTask;
    }

    public Task RecordEventAsync(string sessionId, IRecordingEvent evt, CancellationToken ct = default)
    {
        RecordingSession? session;
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out session))
            {
                throw new InvalidOperationException($"No active recording for session {sessionId}");
            }
        }

        var json = JsonSerializer.Serialize(evt, evt.GetType(), new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        session.Writer.WriteLine(json);
        session.EventCount++;

        return Task.CompletedTask;
    }

    public Task<string> StopRecordingAsync(string sessionId, CancellationToken ct = default)
    {
        RecordingSession? session;
        lock (_lock)
        {
            if (!_sessions.TryGetValue(sessionId, out session))
            {
                throw new InvalidOperationException($"No active recording for session {sessionId}");
            }

            _sessions.Remove(sessionId);
        }

        session.Writer.Flush();
        session.Writer.Dispose();

        return Task.FromResult(session.FilePath);
    }

    public bool IsRecording(string sessionId)
    {
        lock (_lock)
        {
            return _sessions.ContainsKey(sessionId);
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            foreach (var session in _sessions.Values)
            {
                session.Writer?.Dispose();
            }
            _sessions.Clear();
        }
    }

    private class RecordingSession
    {
        public string SessionId { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTimeOffset StartTime { get; set; }
        public StreamWriter Writer { get; set; } = null!;
        public int EventCount { get; set; }
    }
}
