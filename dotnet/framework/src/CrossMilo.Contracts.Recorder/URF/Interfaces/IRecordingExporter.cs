using System.Threading;
using System.Threading.Tasks;

namespace Plate.CrossMilo.Contracts.Recorder.URF;

/// <summary>
/// Exports URF recordings to specialized formats (Asciinema, video, etc.)
/// </summary>
public interface IRecordingExporter
{
    /// <summary>Format name (e.g., "asciinema", "sadconsole", "video")</summary>
    string FormatName { get; }

    /// <summary>File extension (e.g., ".cast", ".mp4")</summary>
    string FileExtension { get; }

    /// <summary>
    /// Export a URF recording to the target format
    /// </summary>
    /// <param name="urfPath">Path to URF recording file</param>
    /// <param name="outputPath">Path for exported file</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Path to exported file</returns>
    Task<string> ExportAsync(string urfPath, string outputPath, CancellationToken ct = default);
}
