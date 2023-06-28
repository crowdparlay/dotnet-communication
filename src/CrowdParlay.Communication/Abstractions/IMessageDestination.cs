namespace CrowdParlay.Communication.Abstractions;

public interface IMessageDestination
{
    public void Publish(Message message);
}