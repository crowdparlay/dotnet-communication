using System.Reflection;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection;

public sealed class RabbitMqCommunicationOptions
{
    public Uri? AmqpServerUrl;
    public readonly HashSet<Assembly> MessageListenersAssemblies = new();
}