using System;

namespace Plate.CrossMilo.Contracts.ECS;

/// <summary>
/// Stable identifier for authoring graph nodes used to resolve runtime entities.
/// </summary>
public readonly record struct AuthoringNodeId(Guid Value)
{
    public static AuthoringNodeId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}
