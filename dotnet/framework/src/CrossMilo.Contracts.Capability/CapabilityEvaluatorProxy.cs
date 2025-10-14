using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Plate.PluginManoi.Contracts;

namespace Plate.CrossMilo.Contracts.Capability;

public sealed class CapabilityEvaluatorProxy : ICapabilityEvaluator
{
    private readonly IRegistry _registry;
    public CapabilityEvaluatorProxy(IRegistry registry) { _registry = registry; }

    public Task<bool> CanPerformAsync(string capability, string workflow, object context, CancellationToken ct = default)
        => _registry.Get<ICapabilityEvaluator>().CanPerformAsync(capability, workflow, context, ct);

    public Task<CapabilityResult> EvaluateAsync(string capability, string workflow, object context, CancellationToken ct = default)
        => _registry.Get<ICapabilityEvaluator>().EvaluateAsync(capability, workflow, context, ct);

    public Task<IReadOnlyList<CapabilityResult>> EvaluateAllAsync(string workflow, object context, CancellationToken ct = default)
        => _registry.Get<ICapabilityEvaluator>().EvaluateAllAsync(workflow, context, ct);
}
