namespace CrowdParlay.Communication.Abstractions;

public interface IMessageDestination
{
    public void Publish(Message message);
    public void Subscribe<TMessage>(IMessageListener<TMessage> listener) where TMessage : Message;
    public void Subscribe<TMessage>(Func<TMessage, Task> handleMessage) where TMessage : Message;
    public void Subscribe(Type messageType, Func<Message, Task> handleMessage);
}