using Plate.PluginManoi.Contracts;

namespace Plate.CrossMilo.Contracts.Audio.Services;

/// <summary>
/// Proxy service for IAudioService.
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
