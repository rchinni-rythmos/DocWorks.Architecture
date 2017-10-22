using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.EventBus.Configuration;
using DocWorks.BuildingBlocks.EventBus.Enumerations;
using DocWorks.BuildingBlocks.EventBus.Implementation;
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

            // Required by WebJobs SDK
            Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetValue<string>("AzureWebJobsStorage") );
            Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetValue<string>("AzureWebJobsDashboard"));
            #endregion

            #region Setup DI
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IEventHandlerRegistry, InMemoryEventHandlerRegistry>();
            serviceCollection.AddSingleton(azureServiceBusSettings);
            serviceCollection.AddSingleton<IEventBus, EventBusServiceBusMessageListener>();
            //serviceCollection.AddTransient<GDriveCreateProjectEventHandler>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            #endregion

            RegisterEventHandlers(serviceProvider);

            // Start listening for Events
            serviceProvider.GetService<IEventBus>().RegisterEventListener();

            var host = new JobHost();
            host.RunAndBlock();
        }

        private static void RegisterEventHandlers(ServiceProvider serviceProvider)
        {
            var eventHandlerRegistry = serviceProvider.GetService<IEventHandlerRegistry>();
            //eventHandlerRegistry.AddEventHandler(EventName.GDriveCreateProject, typeof(GDriveCreateProjectEventHandler));
        }
    }
}
