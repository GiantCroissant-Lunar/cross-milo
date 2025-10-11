using System;

namespace Plate.CrossMilo.Contracts;

/// <summary>
/// Marks a partial class as a proxy service that realizes a specific interface.
/// Source generator will implement all interface methods by delegating to the registry.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RealizeServiceAttribute : Attribute
{
    public Type ServiceType { get; }

    public RealizeServiceAttribute(Type serviceType)
    {
        ServiceType = serviceType;
    }
}

/// <summary>
/// Specifies the selection strategy for retrieving service implementations from the registry.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SelectionStrategyAttribute : Attribute
{
    public SelectionMode Mode { get; }

    public SelectionStrategyAttribute(SelectionMode mode)
    {
        Mode = mode;
    }
}
