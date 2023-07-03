using CrowdParlay.Communication.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection;

public sealed class MessageListenersInitializer : IHostedService
{
    private readonly MessageListenersConfiguration _configuration;
    private readonly IMessageListener<Message> _commonListener;
    private readonly IServiceScopeFactory _scopeFactory;

    public MessageListenersInitializer(
        MessageListenersConfiguration configuration, IMessageListener<Message> commonListener, IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _commonListener = commonListener;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var broker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        
        foreach (var messageType in _configuration.SubscribedMessageTypes)
            broker.Users.Subscribe(messageType, _commonListener.HandleAsync);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}