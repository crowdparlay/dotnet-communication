using CrowdParlay.Communication.Abstractions;
using RabbitMQ.Client;

namespace CrowdParlay.Communication.RabbitMq;

public sealed class RabbitMqMessageBroker : IMessageBroker
{
    public IMessageDestination Users { get; }

    public RabbitMqMessageBroker(IConnectionFactory connectionFactory) =>
        Users = new RabbitMqExchange(RabbitMqConstants.Exchanges.Users, connectionFactory);
}