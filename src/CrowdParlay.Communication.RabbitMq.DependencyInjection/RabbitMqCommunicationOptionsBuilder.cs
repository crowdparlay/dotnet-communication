using System.Reflection;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection;

public sealed class RabbitMqCommunicationOptionsBuilder
{
    private readonly RabbitMqCommunicationOptions _options = new();

    public RabbitMqCommunicationOptionsBuilder UseAmqpServer(string amqpServerUrl)
    {
        _options.AmqpServerUrl = new Uri(amqpServerUrl);
        return this;
    }
    
    public RabbitMqCommunicationOptionsBuilder UseMessageListenersFromAssembly(Assembly assembly)
    {
        _options.MessageListenersAssemblies.Add(assembly);
        return this;
    }

    public RabbitMqCommunicationOptions Build()
    {
        if (_options.AmqpServerUrl is null)
            throw new InvalidOperationException(
                "RabbitMQ communication services require AMQP server URL to be configured. Consider calling " +
                $"'{nameof(RabbitMqCommunicationOptionsBuilder)}.{nameof(UseAmqpServer)}' method while configuring your DI container.");

        return _options;
    }
}