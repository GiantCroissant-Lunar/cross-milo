using System;
using Microsoft.Extensions.Logging;
using Plate.PluginManoi.Contracts;
using Plate.Goap.Core;
using Plate.Goap.Resolver;

namespace Plate.CrossMilo.Contracts.Goap;

public sealed class GoapPlannerProxy : IGoapPlannerService
{
    private readonly IRegistry _registry;

    public GoapPlannerProxy(IRegistry registry)
    {
        _registry = registry;
    }

    public JobResult Plan(IConnectable[] actions, IKeyResolver keyResolver, Func<IGraphResolver, RunData> configure, ILogger? logger = null)
    {
        var svc = _registry.Get<IGoapPlannerService>();
        return svc.Plan(actions, keyResolver, configure, logger);
    }

    public void PlanDemo(ILogger? logger = null)
    {
        var svc = _registry.Get<IGoapPlannerService>();
        svc.PlanDemo(logger);
    }
}
