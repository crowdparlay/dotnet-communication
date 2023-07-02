namespace CrowdParlay.Communication.RabbitMq.DependencyInjection;

public sealed class MessageListenersConfiguration
{
    public required IEnumerable<Type> SubscribedMessageTypes;
}