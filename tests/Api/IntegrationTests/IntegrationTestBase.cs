using Alba;
using Marten;

namespace Orders.Api.IntegrationTests;

[Collection(ScenarioCollection.CollectionName)]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected IntegrationTestBase(WebApplicationFixture fixture)
    {
        Host = fixture.Host;
        Store = Host.DocumentStore();
    }

    public IAlbaHost Host { get; }

    public IDocumentStore Store { get; }

    public virtual async Task InitializeAsync()
    {
        await Store.Advanced.ResetAllData();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
