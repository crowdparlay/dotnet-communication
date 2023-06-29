using CrowdParlay.Communication.Abstractions;

namespace CrowdParlay.Communication.RabbitMq.IntegrationTests.Services;

public class UserEventsAwaitableConsumer : IMessageListener<UserCreatedEvent>, IMessageListener<UserUpdatedEvent>, IMessageListener<UserDeletedEvent>
{
    private TaskCompletionSource<Message> _tcs = new();

    public async Task<Message> ConsumeOne()
    {
        var message = await _tcs.Task;
        _tcs = new TaskCompletionSource<Message>();
        return message;
    }

    public Task HandleAsync(UserCreatedEvent message)
    {
        _tcs.SetResult(message);
        return Task.CompletedTask;
    }

    public Task HandleAsync(UserUpdatedEvent message)
    {
        _tcs.SetResult(message);
        return Task.CompletedTask;
    }

    public Task HandleAsync(UserDeletedEvent message)
    {
        _tcs.SetResult(message);
        return Task.CompletedTask;
    }
}