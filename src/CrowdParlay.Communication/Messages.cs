namespace CrowdParlay.Communication;

public record UserCreatedEvent(string UserId, string Username, string DisplayName, string? AvatarUrl);
public record UserUpdatedEvent(string UserId, string Username, string DisplayName, string? AvatarUrl);
public record UserDeletedEvent(string UserId);