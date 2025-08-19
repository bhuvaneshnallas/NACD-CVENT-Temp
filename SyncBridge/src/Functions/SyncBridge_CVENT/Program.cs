using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SyncBridge.Application.UseCases;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Infrastructure.Data;
using SyncBridge.Infrastructure.Services;
using AutoMapper;
using SyncBridge.Infrastructure.AutoMapper;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


var host = new HostBuilder()
    .ConfigureAppConfiguration(
        (context, configBuilder) =>
        {
            var rootPath = context.HostingEnvironment.ContentRootPath;
            configBuilder
                .SetBasePath(rootPath)
                //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
        )

    .ConfigureFunctionsWebApplication()
    .ConfigureServices(
        (context, services) =>
        {
            var config = context.Configuration;
            var connectionString = config["CosmosDbConfig:ConnectionString"];
            var databaseName = config["CosmosDbConfig:SyncDatabase"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("CosmosDbConfig:ConnectionString is missing in configuration.");

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new InvalidOperationException("CosmosDbConfig:SyncDatabase is missing in configuration.");

            services.AddDbContextFactory<SyncLogDBContext>(options =>
            {
                options.UseCosmos(connectionString, databaseName);
            });

            services.AddHttpClient();
            services.AddApplicationInsightsTelemetryWorkerService();
            services.ConfigureFunctionsApplicationInsights();

            services.AddScoped<SyncEventGridToSalesForce>();
            services.AddScoped<IEventGridService, EventGridService>();
            services.AddScoped<ISalesforceService, SalesForceService>();
            services.AddScoped<ISyncLogService,SyncLogService>();


            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            },new LoggerFactory());
            services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
        }
    )
    .Build();

host.Run();



//services.AddTransient<ICventService, CventService>();
//services.AddTransient<SyncCVENTToCosmos>();


//services.AddScoped<ISyncLogService, SyncLogService>();
//services.AddScoped<ISyncEventPublisher, SyncEventPublisher>();
//services.AddLogging();
