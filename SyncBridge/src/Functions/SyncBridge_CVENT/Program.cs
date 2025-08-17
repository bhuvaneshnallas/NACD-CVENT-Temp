using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SyncBridge.Application.UseCases;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Infrastructure.Data;
using SyncBridge.Infrastructure.Data.Repository;
using SyncBridge.Infrastructure.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddTransient<ICventService, CventService>();
builder.Services.AddTransient<IEventsDbRepo, EventsDbRepo>();

builder.Services.AddTransient<SyncCVENTToCosmos>();
builder.Services.AddTransient<IEventServices, EventServices>();

var connectionString = builder.Configuration["EventDB:CosmosDBConnectionString"];
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("CosmosDB connection string is not configured.");
}
var databaseName = builder.Configuration["EventDB:DatabaseName"];
if (string.IsNullOrEmpty(databaseName))
{
    throw new InvalidOperationException("CosmosDB database name is not configured.");
}

builder.Services.AddDbContextFactory<EventsDbContext>(options =>
{
    options.UseCosmos(
        connectionString,
        databaseName
    );
});

CosmosClientOptions options = new CosmosClientOptions()
{
    AllowBulkExecution = true
};

CosmosClient cosmosClient = new CosmosClient(connectionString, options);
builder.Services.AddSingleton(cosmosClient);

// We cannot use DbContext directly, so we use the interface
builder.Services.AddScoped<IEventsDbContext>(provider => provider.GetRequiredService<EventsDbContext>());

builder.ConfigureFunctionsWebApplication();

builder.Services.AddApplicationInsightsTelemetryWorkerService().ConfigureFunctionsApplicationInsights();

builder.Build().Run();
