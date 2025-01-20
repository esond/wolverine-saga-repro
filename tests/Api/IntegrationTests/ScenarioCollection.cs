namespace Orders.Api.IntegrationTests;

[CollectionDefinition(CollectionName, DisableParallelization = true)]
public class ScenarioCollection : ICollectionFixture<WebApplicationFixture>
{
    public const string CollectionName = "scenarios";
}
