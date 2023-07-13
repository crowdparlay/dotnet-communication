using CrowdParlay.Communication.Abstractions;

namespace CrowdParlay.Communication;

public record UserCreatedEvent(string UserId, string Username, string DisplayName, string? AvatarUrl) : Message;
public record UserUpdatedEvent(string UserId, string Username, string DisplayName, string? AvatarUrl) : Message;
public record UserDeletedEvent(string UserId) : Message;