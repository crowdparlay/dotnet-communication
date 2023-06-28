using System.Text;
using CrowdParlay.Communication.Abstractions;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace CrowdParlay.Communication.RabbitMq;

public class RabbitMqExchange : IMessageDestination
{
    private readonly string _exchange;
    private readonly IConnectionFactory _connectionFactory;

    public RabbitMqExchange(string exchange, IConnectionFactory connectionFactory)
    {
        _exchange = exchange;
        _connectionFactory = connectionFactory;
    }

    public void Publish(Message message)
    {
        var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        
        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);
        var routingKey = ResolveRoutingKey(message);
        
        channel.BasicPublish(_exchange, routingKey, body: body);
    }
    
    private string ResolveRoutingKey(Message message) => message switch
    {
        UserCreatedEvent => RabbitMqConstants.RoutingKeys.UserCreated,
        UserUpdatedEvent => RabbitMqConstants.RoutingKeys.UserUpdated,
        UserDeletedEvent => RabbitMqConstants.RoutingKeys.UserDeleted,
        _ => throw new NotSupportedException($"No corresponding RabbitMQ routing key found for message of type '{message.GetType().FullName}'.")
    };
}