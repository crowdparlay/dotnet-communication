namespace CrowdParlay.Communication.RabbitMq.IntegrationTests.Attributes;

public class SetupAttribute<TSetup> : AutoDataAttribute where TSetup : ICustomization, new()
{
    public SetupAttribute() : base(() => new Fixture()
        .Customize(new TSetup())) { }
}