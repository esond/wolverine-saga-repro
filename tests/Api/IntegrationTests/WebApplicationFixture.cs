using Alba;
using Microsoft.AspNetCore.Hosting;
using Oakton;
using Testcontainers.PostgreSql;

namespace Orders.Api.IntegrationTests;

public class WebApplicationFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public IAlbaHost Host { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        OaktonEnvironment.AutoStartHost = true;

        Host = await AlbaHost.For<Program>(ConfigureWebHost);
    }

    private void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:postgresdb", _dbContainer.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await Host.StopAsync();
        await _dbContainer.StopAsync();
    }
}
