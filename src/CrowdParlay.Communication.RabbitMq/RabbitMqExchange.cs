using System.Text;
using CrowdParlay.Communication.Abstractions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CrowdParlay.Communication.RabbitMq;

public class RabbitMqExchange : IMessageDestination, IDisposable
{
    private readonly string _exchange;
    private readonly IModel _channel;

    private readonly Dictionary<Type, string> _routingKeysByMessageType = new()
    {
        [typeof(UserCreatedEvent)] = RabbitMqConstants.RoutingKeys.UserCreated,
        [typeof(UserUpdatedEvent)] = RabbitMqConstants.RoutingKeys.UserUpdated,
        [typeof(UserDeletedEvent)] = RabbitMqConstants.RoutingKeys.UserDeleted
    };

    public RabbitMqExchange(string exchange, IConnectionFactory connectionFactory)
    {
        _exchange = exchange;
        var connection = connectionFactory.CreateConnection();
        _channel = connection.CreateModel();
    }

    public void Publish(Message message)
    {
        _channel.ExchangeDeclare(_exchange, ExchangeType.Topic);
        
        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);
        var routingKey = ResolveRoutingKey(message.GetType());

        _channel.BasicPublish(_exchange, routingKey, body: body);
    }

    public void Subscribe<TMessage>(IMessageListener<TMessage> listener) where TMessage : Message
    {
        _channel.ExchangeDeclare(_exchange, ExchangeType.Topic);
        
        var routingKey = ResolveRoutingKey(typeof(TMessage));
        var queue = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue, _exchange, routingKey);
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            _channel.BasicAck(args.DeliveryTag, multiple: false);

            var body = Encoding.UTF8.GetString(args.Body.ToArray());
            var message = JsonConvert.DeserializeObject<TMessage>(body)!;
            await listener.HandleAsync(message);
        };

        _channel.BasicConsume(queue, autoAck: false, consumer);
    }

    private string ResolveRoutingKey(Type messageType)
    {
        if (!_routingKeysByMessageType.TryGetValue(messageType, out var routingKey))
            throw new NotSupportedException($"No corresponding RabbitMQ routing key found for message of type '{messageType.FullName}'.");

        return routingKey;
    }

    public void Dispose() => _channel.Dispose();
}