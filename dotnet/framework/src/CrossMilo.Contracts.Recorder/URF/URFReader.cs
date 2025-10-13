using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;

namespace Plate.CrossMilo.Contracts.Recorder.URF;

/// <summary>
/// Reads and parses URF recording files
/// </summary>
public static class URFReader
{
    /// <summary>
    /// Load all events from a URF recording file
    /// </summary>
    public static List<IRecordingEvent> Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"URF recording not found: {path}");
        }

        var events = new List<IRecordingEvent>();
        var lines = File.ReadAllLines(path);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            try
            {
                var doc = JsonDocument.Parse(line);
                var type = doc.RootElement.GetProperty("type").GetString();

                var evt = type switch
                {
                    "meta" => JsonSerializer.Deserialize<MetadataEvent>(line) as IRecordingEvent,
                    "output" => JsonSerializer.Deserialize<OutputEvent>(line) as IRecordingEvent,
                    "state" => JsonSerializer.Deserialize<StateEvent>(line) as IRecordingEvent,
                    "input" => JsonSerializer.Deserialize<InputEvent>(line) as IRecordingEvent,
                    _ => null
                };

                if (evt != null)
                {
                    events.Add(evt);
                }
            }
            catch (Exception ex)
            {
                // Log and skip malformed lines
                Console.WriteLine($"Warning: Failed to parse line: {ex.Message}");
            }
        }

        return events;
    }

    /// <summary>
    /// Get events of a specific type
    /// </summary>
    public static IEnumerable<T> GetEvents<T>(this List<IRecordingEvent> events)
        where T : IRecordingEvent
        => events.OfType<T>();

    /// <summary>
    /// Get metadata from recording
    /// </summary>
    public static MetadataEvent? GetMetadata(this List<IRecordingEvent> events)
        => events.OfType<MetadataEvent>().FirstOrDefault();

    /// <summary>
    /// Get events in a time range
    /// </summary>
    public static IEnumerable<IRecordingEvent> GetEventsInRange(
        this List<IRecordingEvent> events,
        double startTime,
        double endTime)
        => events.Where(e => e.Timestamp >= startTime && e.Timestamp <= endTime);

    /// <summary>
    /// Get total recording duration
    /// </summary>
    public static double GetDuration(this List<IRecordingEvent> events)
    {
        if (events.Count == 0)
        {
            return 0;
        }

        return events.Max(e => e.Timestamp);
    }

    /// <summary>
    /// Get event count by type
    /// </summary>
    public static Dictionary<string, int> GetEventCounts(this List<IRecordingEvent> events)
    {
        return events
            .GroupBy(e => e.Type)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
