using CrowdParlay.Communication.Abstractions;

namespace CrowdParlay.Communication;

public record UserCreatedEvent(string UserId, string Username, string DisplayName) : Message;
public record UserUpdatedEvent(string UserId, string Username, string DisplayName) : Message;
public record UserDeletedEvent(string UserId) : Message;