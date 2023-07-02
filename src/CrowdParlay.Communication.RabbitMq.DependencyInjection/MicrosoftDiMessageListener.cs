using CrowdParlay.Communication.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection;

internal sealed class MicrosoftDiMessageListener : IMessageListener<Message>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MicrosoftDiMessageListener(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task HandleAsync(Message message)
    {
        var closedGenericListenerInterface = typeof(IMessageListener<>).MakeGenericType(message.GetType());
        var listenerHandleAsyncMethod = closedGenericListenerInterface.GetMethod(nameof(IMessageListener<Message>.HandleAsync))!;
        
        using var scope = _scopeFactory.CreateScope();
        var listener = scope.ServiceProvider.GetRequiredService(closedGenericListenerInterface);

        await (Task)listenerHandleAsyncMethod.Invoke(listener, new object?[] { message })!;
    }
}