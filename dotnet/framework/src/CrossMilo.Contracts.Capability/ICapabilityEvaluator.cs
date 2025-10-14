using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Plate.CrossMilo.Contracts.Capability;

public interface ICapabilityEvaluator
{
    Task<bool> CanPerformAsync(string capability, string workflow, object context, CancellationToken ct = default);
    Task<CapabilityResult> EvaluateAsync(string capability, string workflow, object context, CancellationToken ct = default);
    Task<IReadOnlyList<CapabilityResult>> EvaluateAllAsync(string workflow, object context, CancellationToken ct = default);
}
