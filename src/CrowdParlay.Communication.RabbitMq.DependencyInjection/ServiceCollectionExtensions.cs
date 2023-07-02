using CrowdParlay.Communication.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqCommunication(
        this IServiceCollection services, Action<RabbitMqCommunicationOptionsBuilder> configureOptions)
    {
        var builder = new RabbitMqCommunicationOptionsBuilder();
        configureOptions(builder);
        var options = builder.Build();

        // Example of what the dictionary will look like after assembly scanning:
        // { MessageA, MessageAListener },
        // { MessageB, MessageBListener },
        // { MessageC, MessageCListener }

        var listenerImplementations = new Dictionary<Type, Type>();
        foreach (var type in options.MessageListenersAssemblies.SelectMany(assembly => assembly.GetTypes()))
        {
            var listenerInterfaces = type.GetInterfaces().Where(@interface =>
                @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IMessageListener<>));

            foreach (var listenerInterface in listenerInterfaces)
            {
                var messageType = listenerInterface.GetGenericArguments().Single();

                if (messageType == typeof(Message))
                    throw new InvalidOperationException(
                        $"Message listener of type '{type}' cannot be registered as handler for messages of type '{messageType}', " +
                        $"since '{typeof(IMessageListener<Message>)}' listener type is reserved for the default message router.");
                
                if (listenerImplementations.TryAdd(messageType, type))
                {
                    services.AddScoped(listenerInterface, type);
                    continue;
                }

                var originalListenerType = listenerImplementations[listenerInterface];
                throw new InvalidOperationException(
                    $"Message listener of type '{type}' cannot be registered as handler for messages of type '{messageType}', " +
                    $"since messages of this type are already handled by message listener of type '{originalListenerType}'.");
            }
        }
        
        return services
            .AddSingleton<IConnectionFactory>(new ConnectionFactory { Uri = options.AmqpServerUrl })
            .AddSingleton(new MessageListenersConfiguration { SubscribedMessageTypes = listenerImplementations.Keys })
            .AddSingleton<IMessageListener<Message>, MicrosoftDiMessageListener>()
            .AddScoped<IMessageBroker, RabbitMqMessageBroker>()
            .AddHostedService<MessageListenersInitializer>()
            .AddSingleton<MessageListenersInitializer>();
    }
}