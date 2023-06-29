using CrowdParlay.Communication.RabbitMq.IntegrationTests.Services;
using CrowdParlay.Communication.RabbitMq.IntegrationTests.Setups;
using FluentAssertions;

namespace CrowdParlay.Communication.RabbitMq.IntegrationTests;

public class InjectRabbitMqAttribute : AutoDataAttribute
{
    public InjectRabbitMqAttribute() : base(() => new Fixture()
        .Customize(new RabbitMqSetup())) { }
}

public class MessageTransferTests
{
    [Theory(Timeout = 5000), InjectRabbitMq]
    public async Task OutcomeEvent_ShouldBeTransferredInto_EquivalentIncomeEvent(RabbitMqMessageBroker broker, UserUpdatedEvent outcomeEvent)
    {
        // Arrange
        var consumer = new UserEventsAwaitableConsumer();
        
        broker.Users.Subscribe<UserCreatedEvent>(consumer);
        broker.Users.Subscribe<UserUpdatedEvent>(consumer);
        broker.Users.Subscribe<UserDeletedEvent>(consumer);
        
        // Act
        broker.Users.Publish(outcomeEvent);
        var incomeEvent = await consumer.ConsumeOne();
        
        // Assert
        incomeEvent.Should().Be(outcomeEvent);
    }
}