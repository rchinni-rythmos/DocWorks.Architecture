﻿using DocWorks.BuildingBlocks.EventBus.Abstractions;
using DocWorks.BuildingBlocks.EventBus.Configuration;
using DocWorks.BuildingBlocks.EventBus.Implementation;
using DocWorks.BuildingBlocks.Global.Enumerations.Events;
using DocWorks.BuildingBlocks.Global.Implementation;
using DocWorks.GDocFactory.Configuration;
using DocWorks.GDocFactory.EventHandlers;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace DocWorks.GDocFactory
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

            var gdriveFactorySettings = new GDriveFactoryAppSettings();
            configuration.GetSection(nameof(GDriveFactoryAppSettings)).Bind(gdriveFactorySettings);

            // Required by WebJobs SDK
            Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetValue<string>("AzureWebJobsStorage"));
            Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetValue<string>("AzureWebJobsDashboard"));
            #endregion

            #region Setup DI
            var serviceCollection = new ServiceCollection();

            // GDrive
            serviceCollection.AddSingleton(gdriveFactorySettings);

            // EventBus
            serviceCollection.AddSingleton(azureServiceBusSettings);
            serviceCollection.AddSingleton<IEventHandlerRegistry, InMemoryEventHandlerRegistry>();
            serviceCollection.AddSingleton<IEventBusMessageListener, EventBusServiceBusMessageListener>();
            serviceCollection.AddTransient<IEventBusMessageProcessor, DefaultEventBusServiceBusMessageProcessor>();
            serviceCollection.AddSingleton<IEventBusMessagePublisher, EventBusServiceBusMessagePublisher>();

            // Event Handlers
            serviceCollection.AddTransient<GDriveCreateProjectEventHandler>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            #endregion

            RegisterEventHandlers(serviceProvider);

            // Start listening for Events
            serviceProvider.GetService<IEventBusMessageListener>().RegisterEventListener();

            // Azure WebJobs SDK .Net Core 2.0 is in Beta with no firm release dates.
            // So, cannot use the ServiceBus Triggers part of the SDK.
            // Just using the below part of SDK code, to keep the console application running continuously. 
            var host = new JobHost();
            host.RunAndBlock();
        }

        private static void RegisterEventHandlers(ServiceProvider serviceProvider)
        {
            var eventHandlerRegistry = serviceProvider.GetService<IEventHandlerRegistry>();
            eventHandlerRegistry.AddEventHandler(EventName.GDriveCreateProject, typeof(GDriveCreateProjectEventHandler));
        }
    }
}
