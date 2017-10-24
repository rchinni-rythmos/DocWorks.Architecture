using DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository;
using DocWorks.BuildingBlocks.DataAccess.Configuration;
using DocWorks.BuildingBlocks.DataAccess.Implementation.Repository;
using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.EventBus.Implementation;
using DocWorks.BuildingBlocks.Global.Abstractions;
using DocWorks.BuildingBlocks.Global.Configuration;
using DocWorks.BuildingBlocks.Global.Enumerations;
using DocWorks.BuildingBlocks.Global.Implementation;
using DocWorks.BuildingBlocks.Notification.Abstractions;
using DocWorks.BuildingBlocks.Notification.Configuration;
using DocWorks.BuildingBlocks.Notification.Implementation;
using DocWorks.DataAccess.Common.Abstractions.Repository;
using DocWorks.DataAccess.Common.Implementation.Repository;
using DocWorks.Notification.EventBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace DocWorks.Notification
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Setup Configuration
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
            IConfigurationRoot configuration = builder.Build();

            var azureServiceBusSettings = new AzureServiceBusSettings();
            configuration.GetSection(nameof(AzureServiceBusSettings)).Bind(azureServiceBusSettings);

            var mongoDBSettings = new MongoDBSettings();
            configuration.GetSection(nameof(MongoDBSettings)).Bind(mongoDBSettings);

            FcmAppSettings fcmAppSettings = new FcmAppSettings();
            configuration.GetSection(nameof(FcmAppSettings)).Bind(fcmAppSettings);

            // Required by WebJobs SDK
            Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetValue<string>("AzureWebJobsStorage") );
            Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetValue<string>("AzureWebJobsDashboard"));
            #endregion

            #region Setup DI
            var serviceCollection = new ServiceCollection();

            // EventBus
            serviceCollection.AddSingleton(azureServiceBusSettings);

            serviceCollection.AddSingleton<IEventBusMessageListener, EventBusServiceBusMessageListener>();
            serviceCollection.AddTransient<IEventBusMessageProcessor, OrchestratorEventBusServiceBusMessageProcessor>();
            serviceCollection.AddSingleton<IEventBusMessagePublisher, EventBusServiceBusMessagePublisher>();

            // DB
            serviceCollection.AddSingleton(mongoDBSettings);
            serviceCollection.AddSingleton<IResponseRepository, ResponseRepository>();

            // Notification
            serviceCollection.AddSingleton(fcmAppSettings);
            serviceCollection.AddSingleton<INotificationService, FcmNotificationService>();

            // Event Handlers
            // None for Notification

            var serviceProvider = serviceCollection.BuildServiceProvider();
            #endregion

            // Start listening for Events
            serviceProvider.GetService<IEventBusMessageListener>().RegisterEventListener();

            var host = new JobHost();
            host.RunAndBlock();
        }
    }
}
