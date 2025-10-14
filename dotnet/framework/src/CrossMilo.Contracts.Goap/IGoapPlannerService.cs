using System;
using Microsoft.Extensions.Logging;
using Plate.Goap.Core;
using Plate.Goap.Resolver;

namespace Plate.CrossMilo.Contracts.Goap;

public interface IGoapPlannerService
{
    JobResult Plan(
        IConnectable[] actions,
        IKeyResolver keyResolver,
        Func<IGraphResolver, RunData> configure,
        ILogger? logger = null);

    void PlanDemo(ILogger? logger = null);
}
