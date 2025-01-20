using System.Text.Json;
using System.Text.Json.Serialization;
using JasperFx.CodeGeneration;
using Marten;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.OpenApi.Models;
using Oakton;
using Oakton.Resources;
using Weasel.Core;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Host.ApplyOaktonExtensions();

builder.Services.AddWolverineHttp();

builder.Host.UseWolverine(opts =>
{
    // Configure Marten integration
    opts.Services.AddMarten(options =>
    {
        options.Connection(builder.Configuration.GetConnectionString("postgresdb")!);

        options.UseSystemTextJsonForSerialization(EnumStorage.AsString, Casing.CamelCase,
            jsonOptions =>
            {
                jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

        options.GeneratedCodeMode = TypeLoadMode.Dynamic;

        options.AutoCreateSchemaObjects = AutoCreate.All;

        options.DisableNpgsqlLogging = true;
    })
    .IntegrateWithWolverine()
    .UseLightweightSessions();

    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Dynamic;

    opts.Policies.AutoApplyTransactions();

    opts.UseSystemTextJsonForSerialization(options =>
    {
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

    // Configure messaging
    // https://wolverinefx.net/guide/handlers/#multiple-handlers-for-the-same-message-type
    opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;

    opts.Services.AddResourceSetupOnStartup();
});

builder.Host.UseResourceSetupOnStartup();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
});

app.MapWolverineEndpoints();

return await app.RunOaktonCommands(args);


namespace Orders.Api
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class Program;
}
