using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Plate.CrossMilo.Contracts.Hosting.Host;

/// <summary>
/// Abstraction for platform-specific hosts.
/// Wraps .NET Generic Host or native platform lifecycle.
/// </summary>
public interface IService
{
    /// <summary>
    /// Run the host and block until shutdown.
    /// </summary>
    Task RunAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Start the host without blocking.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stop the host gracefully.
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Access to DI container.
    /// </summary>
    IServiceProvider Services { get; }
}

/// <summary>
/// Builder for configuring Winged Bean hosts.
/// </summary>
public interface IServiceBuilder
{
    IServiceBuilder ConfigureServices(Action<IServiceCollection> configure);
    IServiceBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configure);
    IServiceBuilder ConfigureLogging(Action<ILoggingBuilder> configure);
    IService Build();
}
