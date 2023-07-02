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

    public void Subscribe<TMessage>(IMessageListener<TMessage> listener) where TMessage : Message =>
        Subscribe<TMessage>(async message => await listener.HandleAsync(message));
    
    public void Subscribe<TMessage>(Func<TMessage, Task> handleMessage) where TMessage : Message =>
        Subscribe(typeof(TMessage), async message => await handleMessage((TMessage)message));
    
    public void Subscribe(Type messageType, Func<Message, Task> handleMessage)
    {
        if (!messageType.IsAssignableTo(typeof(Message)))
            throw new ArgumentException($"Only types derived from '{typeof(Message)}' can be used as message types.", nameof(messageType));
        
        _channel.ExchangeDeclare(_exchange, ExchangeType.Topic);

        var routingKey = ResolveRoutingKey(messageType);
        var queue = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue, _exchange, routingKey);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            var body = Encoding.UTF8.GetString(args.Body.ToArray());
            var message = (Message)JsonConvert.DeserializeObject(body, messageType)!;
            await handleMessage(message);
            
            _channel.BasicAck(args.DeliveryTag, multiple: false);
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