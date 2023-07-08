using Nito.AsyncEx;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;

namespace CrowdParlay.Communication.RabbitMq.IntegrationTests.Setups;

public class RabbitMqSetup : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var rabbitMqContainer = new RabbitMqBuilder().Build();
        AsyncContext.Run(async () => await rabbitMqContainer.StartAsync());

        var amqpServerUrl = rabbitMqContainer.GetConnectionString();
        var connectionFactory = new ConnectionFactory { Uri = new Uri(amqpServerUrl) };
        var broker = new RabbitMqMessageBroker(connectionFactory);
        fixture.Inject(broker);
    }
}