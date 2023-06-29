namespace CrowdParlay.Communication.Abstractions;

public interface IMessageListener<in TMessage> where TMessage : Message
{
    public Task HandleAsync(TMessage message);
}