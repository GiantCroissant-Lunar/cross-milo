using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;

namespace Plate.CrossMilo.Contracts.Recorder.URF.Exporters;

/// <summary>
/// Exports URF recordings to Asciinema v2 format
/// </summary>
public class AsciinemaExporter : IRecordingExporter
{
    public string FormatName => "asciinema";
    public string FileExtension => ".cast";

    public async Task<string> ExportAsync(string urfPath, string outputPath, CancellationToken ct = default)
    {
        if (!File.Exists(urfPath))
        {
            throw new FileNotFoundException($"URF file not found: {urfPath}");
        }

        var lines = await File.ReadAllLinesAsync(urfPath, ct);
        if (lines.Length == 0)
        {
            throw new InvalidOperationException("URF file is empty");
        }

        // Parse metadata event (first line)
        var metaEvent = JsonSerializer.Deserialize<MetadataEvent>(lines[0]);
        if (metaEvent == null || metaEvent.Type != "meta")
        {
            throw new InvalidOperationException("First event must be metadata event");
        }

        var outputLines = new List<string>();

        // Create Asciinema v2 header
        var header = new
        {
            version = 2,
            width = metaEvent.Recording.Terminal?.Width ?? 80,
            height = metaEvent.Recording.Terminal?.Height ?? 24,
            timestamp = metaEvent.Recording.Started.ToUnixTimeSeconds(),
            title = metaEvent.Recording.Application.Name,
            env = new Dictionary<string, string>
            {
                ["TERM"] = "xterm-256color",
                ["SHELL"] = "/bin/bash"
            }
        };

        outputLines.Add(JsonSerializer.Serialize(header));

        // Convert output events to Asciinema format
        for (int i = 1; i < lines.Length; i++)
        {
            try
            {
                var jsonDoc = JsonDocument.Parse(lines[i]);
                var root = jsonDoc.RootElement;

                var eventType = root.GetProperty("type").GetString();
                if (eventType != "output")
                {
                    continue;
                }

                var stream = root.TryGetProperty("stream", out var streamProp) ? streamProp.GetString() : null;
                if (stream != "terminal")
                {
                    continue;
                }

                var t = root.GetProperty("t").GetDouble();
                var data = root.GetProperty("data");

                string? outputText = null;
                if (data.TryGetProperty("text", out var textProp))
                {
                    outputText = textProp.GetString();
                }
                else if (data.TryGetProperty("bytes", out var bytesProp))
                {
                    var base64 = bytesProp.GetString();
                    if (!string.IsNullOrEmpty(base64))
                    {
                        var bytes = Convert.FromBase64String(base64);
                        outputText = Encoding.UTF8.GetString(bytes);
                    }
                }

                if (!string.IsNullOrEmpty(outputText))
                {
                    // Asciinema format: [time, "o", "data"]
                    var asciiEvent = new object[] { t, "o", outputText };
                    outputLines.Add(JsonSerializer.Serialize(asciiEvent));
                }
            }
            catch
            {
                // Skip malformed events
                continue;
            }
        }

        await File.WriteAllLinesAsync(outputPath, outputLines, ct);
        return outputPath;
    }
}
