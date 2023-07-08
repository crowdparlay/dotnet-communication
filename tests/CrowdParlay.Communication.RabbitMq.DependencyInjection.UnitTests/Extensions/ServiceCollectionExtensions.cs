using Microsoft.Extensions.DependencyInjection;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection.UnitTests.Extensions;

public static class ServiceCollectionExtensions
{
    public static bool Contains<TService>(this IServiceCollection services) =>
        services.Any(descriptor => descriptor.ServiceType == typeof(TService));
}