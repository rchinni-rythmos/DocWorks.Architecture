using Autofac;
using Autofac.Extensions.DependencyInjection;
using DocWorks.BuildingBlocks.DataAccess.Abstractions;
using DocWorks.BuildingBlocks.DataAccess.Abstractions.Repository;
using DocWorks.BuildingBlocks.DataAccess.Configuration;
using DocWorks.BuildingBlocks.DataAccess.Implementation;
using DocWorks.BuildingBlocks.DataAccess.Implementation.Repository;
using DocWorks.BuildingBlocks.Global.Abstractions;
using DocWorks.BuildingBlocks.Global.Configuration;
using DocWorks.BuildingBlocks.Global.Implementation;
using DocWorks.BuildingBlocks.Notification.Abstractions;
using DocWorks.BuildingBlocks.Notification.Configuration;
using DocWorks.BuildingBlocks.Notification.Implementation;
using DocWorks.CMS.Api.Authentication;
using DocWorks.CMS.Api.Configuration;
using DocWorks.CMS.Api.Infrastructure.Filter;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;

namespace DocWorks.CmsApi
{
    public class Startup
    {
        AzureServiceBusSettings azureServiceBusSettings = null;
        MongoDBSettings mongoDBSettings = null;
        FcmAppSettings fcmAppSettings = null;
        AuthenticationSettings authenticationSettings = null;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            this.azureServiceBusSettings = new AzureServiceBusSettings();
            configuration.GetSection(nameof(AzureServiceBusSettings)).Bind(azureServiceBusSettings);
            this.mongoDBSettings = new MongoDBSettings();
            configuration.GetSection(nameof(MongoDBSettings)).Bind(mongoDBSettings);
            this.fcmAppSettings = new FcmAppSettings();
            configuration.GetSection(nameof(FcmAppSettings)).Bind(fcmAppSettings);
            this.authenticationSettings = new AuthenticationSettings();
            Configuration.GetSection(nameof(AuthenticationSettings)).Bind(authenticationSettings);
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            // ExpandoObject (the "content" in Response Entity ) is not serialized to camel case by default.
            // So set the resolver
            // https://stackoverflow.com/questions/41329279/net-core-json-serialization-of-properties-on-dynamic-expandoobject
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                options.Filters.Add(typeof(ValidateModelStateFilter));
            }).AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            // Register IdentityServer
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                //.AddInMemoryIdentityResources(AuthenticationConfig.GetIdentityResources())
                .AddInMemoryApiResources(AuthenticationConfig.GetApiResources())
                .AddInMemoryClients(AuthenticationConfig.GetClients())
                .AddCustomUserStore();

            // Register authentication
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = AuthenticationSettings.WebAppURL;
                        options.RequireHttpsMetadata = false;
                        options.ApiName = "CMSApi";
                    });

            var builder = new ContainerBuilder();
            //Services
            services.AddSingleton<IDbService, DbService>();
            services.AddSingleton<IEventBusMessagePublisher, EventBusServiceBusMessagePublisher>();
            services.AddSingleton(this.mongoDBSettings);
            services.AddSingleton(this.azureServiceBusSettings);
            services.AddSingleton(this.fcmAppSettings);
            // TODO - Not sure authenticationSettings can be injected everywhere
            //services.AddSingleton(this.authenticationSettings);
            services.AddSingleton<IRegistrationService, FcmRegistrationService>();

            //Repository
            services.AddSingleton<IResponseRepository, ResponseRepository>();
            services.AddSingleton<IFlowMapRepository, FlowMapRepository>();

            builder.Populate(services);
            this.ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseMvc();
            applicationLifetime.ApplicationStopped.Register(() => this.ApplicationContainer.Dispose());
        }
    }
}
