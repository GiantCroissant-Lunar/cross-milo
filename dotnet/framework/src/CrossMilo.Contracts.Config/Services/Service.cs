using Plate.CrossMilo.Contracts;

namespace Plate.CrossMilo.Contracts.Config.Services;

/// <summary>
/// Proxy service for IConfigService (partial class, source gen fills in methods).
/// </summary>
[RealizeService(typeof(IService))]
[SelectionStrategy(SelectionMode.HighestPriority)]
public partial class ProxyService : IService
{
    private readonly IRegistry _registry;

    /// <summary>
    /// Initializes a new instance of the ProxyService class.
    /// </summary>
    /// <param name="registry">The registry to use for service resolution</param>
    public ProxyService(IRegistry registry)
    {
        _registry = registry;
    }

    // Source generator will implement all interface methods below
}
