# Crowd Parlay's .NET *communication* tools
 
### Technologies
`C#` `.NET 7` `MassTransit` `RabbitMQ`
 
### Responsibilities
- Platform-wide messaging types
- Common MassTransit configuration
- RabbitMQ topology maintenance
 
### Integration
Given `MassTransit.RabbitMQ` NuGet package and `IServiceCollection services` instance, `CrowdParlay.Communication` gets integrated as follows:

```csharp
using CrowdParlay.Communication;
using MassTransit;
```

```csharp
services.AddMassTransit(bus => bus.UsingRabbitMq((context, configurator) =>
{
    var amqpServerUrl =
        configuration["RABBITMQ_AMQP_SERVER_URL"]
        ?? throw new InvalidOperationException("Missing required configuration 'RABBITMQ_AMQP_SERVER_URL'.");

    configurator.Host(amqpServerUrl);
    configurator.ConfigureEndpoints(context);
    configurator.ConfigureTopology(); // This extension method is defined in CrowdParlay.Communication
}));
```

Now, inject `IPublishEndpoint _broker` to publish messages like that:

```csharp
// UserCreatedEvent type is defined in CrowdParlay.Communication
var @event = new UserCreatedEvent(user.Id.ToString(), user.Username, user.DisplayName, user.AvatarUrl);
await _broker.Publish(@event, cancellationToken);
```
