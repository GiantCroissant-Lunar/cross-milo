namespace Plate.CrossMilo.Contracts;

/// <summary>
/// Metadata for a registered service.
/// </summary>
public class ServiceMetadata
{
    public int Priority { get; set; }
    public string? Name { get; set; }
    public string? Version { get; set; }
}
