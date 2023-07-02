using CrowdParlay.Communication.Abstractions;
using Microsoft.Extensions.Hosting;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection;

public sealed class MessageListenersInitializer : IHostedService
{
    private readonly MessageListenersConfiguration _configuration;
    private readonly IMessageBroker _broker;
    private readonly IMessageListener<Message> _commonListener;

    public MessageListenersInitializer(
        MessageListenersConfiguration configuration, IMessageBroker broker, IMessageListener<Message> commonListener)
    {
        _configuration = configuration;
        _broker = broker;
        _commonListener = commonListener;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var messageType in _configuration.SubscribedMessageTypes)
            _broker.Users.Subscribe(messageType, _commonListener.HandleAsync);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}