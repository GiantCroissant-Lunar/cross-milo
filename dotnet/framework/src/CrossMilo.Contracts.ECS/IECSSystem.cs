namespace Plate.CrossMilo.Contracts.ECS;

/// <summary>
/// Base interface for ECS system implementations.
/// Systems execute logic each frame on entities matching their query criteria.
/// </summary>
/// <remarks>
/// This interface defines the system lifecycle contract for Entity Component System (ECS) implementations.
/// Systems are registered with an <see cref="Services.IService"/> and execute logic each frame.
/// </remarks>
public interface IECSSystem
{
    /// <summary>
    /// Execute system logic for this frame.
    /// </summary>
    /// <param name="ecs">The ECS service providing access to entities and components.</param>
    /// <param name="world">The world this system should operate on.</param>
    /// <param name="deltaTime">Time elapsed since the last frame in seconds.</param>
    void Execute(Services.IService ecs, IWorld world, float deltaTime);
}
