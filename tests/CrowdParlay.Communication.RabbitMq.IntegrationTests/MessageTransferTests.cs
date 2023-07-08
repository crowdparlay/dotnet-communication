using CrowdParlay.Communication.Abstractions;
using CrowdParlay.Communication.RabbitMq.IntegrationTests.Attributes;
using CrowdParlay.Communication.RabbitMq.IntegrationTests.Props;
using CrowdParlay.Communication.RabbitMq.IntegrationTests.Setups;
using FluentAssertions;

namespace CrowdParlay.Communication.RabbitMq.IntegrationTests;

public sealed class MessageTransferTests
{
    [Theory(Timeout = 5000), Setup<RabbitMqSetup>]
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