using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SyncBridge.Application.UseCases;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Infrastructure.Data;
using SyncBridge.Infrastructure.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Services.AddTransient<ICventService, CventService>();
builder.Services.AddTransient<SyncCVENTToCosmos>();

builder.Services.AddDbContextFactory<EventsDbContext>(options =>
{
    options.UseCosmos(
        "_____",
        "mydb"
    );
});

// We cannot use DbContext directly, so we use the interface
builder.Services.AddScoped<IEventsDbContext>(provider => provider.GetRequiredService<EventsDbContext>());

builder.ConfigureFunctionsWebApplication();

builder.Services.AddApplicationInsightsTelemetryWorkerService().ConfigureFunctionsApplicationInsights();

builder.Build().Run();
