namespace Plate.CrossMilo.Contracts.Capability;

public sealed record CapabilityResult(
    string CapabilityName,
    bool CanPerform,
    string? Reason = null,
    float Score = 0f
);
