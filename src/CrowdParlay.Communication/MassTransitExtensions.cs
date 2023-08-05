using MassTransit;

namespace CrowdParlay.Communication;

public static class MassTransitExtensions
{
    public static void ConfigureTopology(this IRabbitMqBusFactoryConfigurator configurator)
    {
        configurator.Message<UserCreatedEvent>(x => x.SetEntityName("user"));
        configurator.Send<UserCreatedEvent>(x => x.UseRoutingKeyFormatter(_ => "user.created"));

        configurator.Message<UserUpdatedEvent>(x => x.SetEntityName("user"));
        configurator.Send<UserUpdatedEvent>(x => x.UseRoutingKeyFormatter(_ => "user.updated"));

        configurator.Message<UserDeletedEvent>(x => x.SetEntityName("user"));
        configurator.Send<UserDeletedEvent>(x => x.UseRoutingKeyFormatter(_ => "user.deleted"));
    }
}