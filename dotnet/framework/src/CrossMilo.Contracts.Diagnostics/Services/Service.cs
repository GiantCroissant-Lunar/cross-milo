using Plate.CrossMilo.Contracts;

namespace Plate.CrossMilo.Contracts.Diagnostics.Services;

/// <summary>
/// Proxy service for IService.
/// </summary>
[RealizeService(typeof(IService))]
[SelectionStrategy(SelectionMode.HighestPriority)]
public partial class ProxyService : IService
{
    private readonly IRegistry _registry;

    public ProxyService(IRegistry registry)
    {
        _registry = registry;
    }

    // Source generator will implement all interface methods below
}
