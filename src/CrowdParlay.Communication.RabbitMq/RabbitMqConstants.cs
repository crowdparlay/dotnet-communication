namespace CrowdParlay.Communication.RabbitMq;

public static class RabbitMqConstants
{
    public static class Exchanges
    {
        public const string Users = "users";
    }

    public static class RoutingKeys
    {
        public const string UserCreated = "users.created";
        public const string UserUpdated = "users.updated";
        public const string UserDeleted = "users.deleted";
    }
}