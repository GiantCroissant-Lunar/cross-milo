using System;
using System.Collections.Generic;
using System.Linq;
using Plate.CrossMilo.Contracts.Recorder.URF.Events;

namespace Plate.CrossMilo.Contracts.Recorder.URF;

/// <summary>
/// Options for comparing recordings
/// </summary>
public class ComparisonOptions
{
    /// <summary>Tolerance for timestamp differences (in seconds)</summary>
    public double TimestampTolerance { get; set; } = 0.1;

    /// <summary>Tolerance for event count differences (percentage, 0-1)</summary>
    public double EventCountTolerance { get; set; } = 0.1;

    /// <summary>Tolerance for duration differences (percentage, 0-1)</summary>
    public double DurationTolerance { get; set; } = 0.15;

    /// <summary>Ignore minor differences in metadata (e.g., timestamps, IDs)</summary>
    public bool IgnoreMetadataVariations { get; set; } = true;

    /// <summary>Compare only event types and counts, not content</summary>
    public bool CompareStructureOnly { get; set; } = false;
}

/// <summary>
/// Result of comparing two recordings
/// </summary>
public class RecordingComparison
{
    public bool IsEquivalent { get; set; }
    public double SimilarityScore { get; set; }
    public List<string> Differences { get; set; } = new();
    public Dictionary<string, int> ActualEventCounts { get; set; } = new();
    public Dictionary<string, int> GoldenEventCounts { get; set; } = new();
    public double ActualDuration { get; set; }
    public double GoldenDuration { get; set; }

    public string Summary => IsEquivalent
        ? $"Recordings are equivalent (similarity: {SimilarityScore:P1})"
        : $"Recordings differ (similarity: {SimilarityScore:P1})\n{string.Join("\n", Differences)}";
}

/// <summary>
/// Compares URF recordings for regression testing
/// </summary>
public class RecordingComparer
{
    private readonly ComparisonOptions _options;

    public RecordingComparer(ComparisonOptions? options = null)
    {
        _options = options ?? new ComparisonOptions();
    }

    /// <summary>
    /// Compare two recordings
    /// </summary>
    public RecordingComparison Compare(List<IRecordingEvent> actual, List<IRecordingEvent> golden)
    {
        var result = new RecordingComparison();
        var differences = new List<string>();
        var scores = new List<double>();

        // Compare event counts by type
        var actualCounts = actual.GetEventCounts();
        var goldenCounts = golden.GetEventCounts();

        result.ActualEventCounts = actualCounts;
        result.GoldenEventCounts = goldenCounts;

        var countScore = CompareEventCounts(actualCounts, goldenCounts, differences);
        scores.Add(countScore);

        // Compare durations
        var actualDuration = actual.GetDuration();
        var goldenDuration = golden.GetDuration();

        result.ActualDuration = actualDuration;
        result.GoldenDuration = goldenDuration;

        var durationScore = CompareDurations(actualDuration, goldenDuration, differences);
        scores.Add(durationScore);

        // Compare metadata (if not ignoring variations)
        if (!_options.IgnoreMetadataVariations)
        {
            var metadataScore = CompareMetadata(actual.GetMetadata(), golden.GetMetadata(), differences);
            scores.Add(metadataScore);
        }

        // Compare event structure (timestamps, order)
        if (!_options.CompareStructureOnly)
        {
            var structureScore = CompareStructure(actual, golden, differences);
            scores.Add(structureScore);
        }

        // Calculate overall similarity score
        result.SimilarityScore = scores.Average();
        result.Differences = differences;
        result.IsEquivalent = result.SimilarityScore >= 0.9; // 90% threshold

        return result;
    }

    /// <summary>
    /// Compare event counts by type
    /// </summary>
    private double CompareEventCounts(
        Dictionary<string, int> actual,
        Dictionary<string, int> golden,
        List<string> differences)
    {
        var allTypes = actual.Keys.Union(golden.Keys).ToList();
        var scores = new List<double>();

        foreach (var type in allTypes)
        {
            var actualCount = actual.GetValueOrDefault(type, 0);
            var goldenCount = golden.GetValueOrDefault(type, 0);

            if (goldenCount == 0 && actualCount > 0)
            {
                differences.Add($"New event type '{type}': {actualCount} events (not in golden)");
                scores.Add(0.5); // Partial credit for new events
                continue;
            }

            if (actualCount == 0 && goldenCount > 0)
            {
                differences.Add($"Missing event type '{type}': expected {goldenCount} events");
                scores.Add(0.0);
                continue;
            }

            var difference = Math.Abs(actualCount - goldenCount);
            var tolerance = goldenCount * _options.EventCountTolerance;

            if (difference > tolerance)
            {
                differences.Add($"Event count mismatch for '{type}': expected ~{goldenCount}, got {actualCount}");
                scores.Add(Math.Max(0, 1.0 - (difference / (double)goldenCount)));
            }
            else
            {
                scores.Add(1.0);
            }
        }

        return scores.Any() ? scores.Average() : 1.0;
    }

    /// <summary>
    /// Compare recording durations
    /// </summary>
    private double CompareDurations(double actual, double golden, List<string> differences)
    {
        if (golden == 0)
        {
            return 1.0; // Can't compare zero duration
        }

        var difference = Math.Abs(actual - golden);
        var tolerance = golden * _options.DurationTolerance;

        if (difference > tolerance)
        {
            differences.Add($"Duration mismatch: expected ~{golden:F2}s, got {actual:F2}s (diff: {difference:F2}s)");
            return Math.Max(0, 1.0 - (difference / golden));
        }

        return 1.0;
    }

    /// <summary>
    /// Compare metadata
    /// </summary>
    private double CompareMetadata(
        MetadataEvent? actual,
        MetadataEvent? golden,
        List<string> differences)
    {
        if (actual == null || golden == null)
        {
            if (actual != golden)
            {
                differences.Add("Metadata presence mismatch");
                return 0.0;
            }
            return 1.0;
        }

        var scores = new List<double>();

        // Compare platform
        if (actual.Recording.Platform != golden.Recording.Platform)
        {
            differences.Add($"Platform mismatch: expected '{golden.Recording.Platform}', got '{actual.Recording.Platform}'");
            scores.Add(0.0);
        }
        else
        {
            scores.Add(1.0);
        }

        // Compare terminal dimensions (if present)
        if (actual.Recording.Terminal != null && golden.Recording.Terminal != null)
        {
            if (actual.Recording.Terminal.Width != golden.Recording.Terminal.Width ||
                actual.Recording.Terminal.Height != golden.Recording.Terminal.Height)
            {
                differences.Add($"Terminal dimensions mismatch: expected {golden.Recording.Terminal.Width}x{golden.Recording.Terminal.Height}, " +
                               $"got {actual.Recording.Terminal.Width}x{actual.Recording.Terminal.Height}");
                scores.Add(0.5);
            }
            else
            {
                scores.Add(1.0);
            }
        }

        return scores.Any() ? scores.Average() : 1.0;
    }

    /// <summary>
    /// Compare event structure (order, timing)
    /// </summary>
    private double CompareStructure(
        List<IRecordingEvent> actual,
        List<IRecordingEvent> golden,
        List<string> differences)
    {
        // Compare event type sequence
        var actualSequence = actual.Select(e => e.Type).ToList();
        var goldenSequence = golden.Select(e => e.Type).ToList();

        // Use longest common subsequence for similarity
        var lcsLength = LongestCommonSubsequence(actualSequence, goldenSequence);
        var maxLength = Math.Max(actualSequence.Count, goldenSequence.Count);
        var sequenceSimilarity = maxLength > 0 ? (double)lcsLength / maxLength : 1.0;

        if (sequenceSimilarity < 0.9)
        {
            differences.Add($"Event sequence differs (similarity: {sequenceSimilarity:P1})");
        }

        return sequenceSimilarity;
    }

    /// <summary>
    /// Longest common subsequence length
    /// </summary>
    private int LongestCommonSubsequence(List<string> a, List<string> b)
    {
        var m = a.Count;
        var n = b.Count;
        var dp = new int[m + 1, n + 1];

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (a[i - 1] == b[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                }
                else
                {
                    dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                }
            }
        }

        return dp[m, n];
    }
}
