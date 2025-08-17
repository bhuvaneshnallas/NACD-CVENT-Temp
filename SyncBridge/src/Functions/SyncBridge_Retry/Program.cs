using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SyncBridge.Domain.Interfaces;
using SyncBridge.Infrastructure.Data;
using SyncBridge.Infrastructure.Services;

var host = new HostBuilder()
    .ConfigureAppConfiguration(
        (context, configBuilder) =>
        {
            var rootPath = context.HostingEnvironment.ContentRootPath;
            configBuilder
                .SetBasePath(rootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            // Key Vault 
            
            var builtConfig = configBuilder.Build();
            var keyVaultName = builtConfig["KeyVaultName"];

            if (string.IsNullOrEmpty(keyVaultName))
            {
                throw new InvalidOperationException("KeyVaultName is not configured in appsettings or environment variables.");
            }

            var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
            configBuilder.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());

            // Key Vault End
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

            services.AddScoped<ICalculatorService, CalculatorService>();
            services.AddScoped<SyncBridge.Application.UseCases.CalculatorUseCase>();
            services.AddSingleton<RetryFailedRecordsUseCase>();
            services.AddScoped<ISyncLogService, SyncLogService>();
            services.AddSingleton<NotificationUseCase>();

            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IRetryService, RetryService>();
            services.AddTransient<ISyncEventPublisher, SyncEventPublisher>();
        }
    )
    .Build();

host.Run();
