using Plate.PluginManoi.Contracts;

namespace Plate.CrossMilo.Contracts.Resource.Services;

/// <summary>
/// Proxy service for IResourceService.
/// Source generator will implement all interface methods by delegating to the registry.
/// </summary>
[RealizeService(typeof(IService))]
[SelectionStrategy(SelectionMode.HighestPriority)]
public partial class ProxyService : IService
{
    private readonly IRegistry _registry;

    /// <summary>
    /// Initializes a new instance of the ProxyService class.
    /// </summary>
    /// <param name="registry">The service registry for resolving implementations.</param>
    public ProxyService(IRegistry registry)
    {
        _registry = registry;
    }

    // Source generator will implement all interface methods below
}
