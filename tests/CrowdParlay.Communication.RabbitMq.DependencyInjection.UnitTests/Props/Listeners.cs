using CrowdParlay.Communication.Abstractions;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection.UnitTests.Props;

public class UserCreatedEventListener : IMessageListener<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent message) => throw new NotSupportedException();
}

internal class UserUpdatedEventListener : IMessageListener<UserUpdatedEvent>
{
    public Task HandleAsync(UserUpdatedEvent message) => throw new NotSupportedException();
}