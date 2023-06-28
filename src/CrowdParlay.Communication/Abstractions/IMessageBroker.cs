namespace CrowdParlay.Communication.Abstractions;

public interface IMessageBroker
{
    public IMessageDestination Users { get; }
}