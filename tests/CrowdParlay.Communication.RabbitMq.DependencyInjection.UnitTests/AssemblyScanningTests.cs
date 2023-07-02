using System.Reflection;
using CrowdParlay.Communication.Abstractions;
using CrowdParlay.Communication.RabbitMq.DependencyInjection.UnitTests.Props;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CrowdParlay.Communication.RabbitMq.DependencyInjection.UnitTests;

public sealed class AssemblyScanningTests
{
    [Fact]
    public void ListenerImplementationsInAssembly_ShouldBe_Registered()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        var services = new ServiceCollection();

        // Act
        services.AddRabbitMqCommunication(options => options
            .UseAmqpServer("amqp:")
            .UseMessageListenersFromAssembly(assembly));

        // Assert
        services
            .FirstOrDefault(x => x.ServiceType == typeof(IMessageListener<Message>))?.ImplementationType
            .Should().Be(typeof(MicrosoftDiMessageListener), "base message type is reserved for default message routing listener");

        services
            .FirstOrDefault(x => x.ServiceType == typeof(IMessageListener<UserCreatedEvent>))?.ImplementationType
            .Should().Be(typeof(UserCreatedEventListener), "the listener implementation type is public");

        services
            .FirstOrDefault(x => x.ServiceType == typeof(IMessageListener<UserUpdatedEvent>))?.ImplementationType
            .Should().Be(typeof(UserUpdatedEventListener), "the listener implementation type is internal");

        services
            .FirstOrDefault(x => x.ServiceType == typeof(IMessageListener<UserDeletedEvent>))?.ImplementationType
            .Should().BeNull("there is no suitable listener implementation type");
    }
}