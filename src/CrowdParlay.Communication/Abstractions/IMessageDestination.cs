namespace CrowdParlay.Communication.Abstractions;

public interface IMessageDestination
{
    public void Publish(Message message);
    public void Subscribe<TMessage>(IMessageListener<TMessage> listener) where TMessage : Message;
}