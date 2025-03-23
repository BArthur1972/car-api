using Cars.ApiCommon.Cosmos.Options;
using Cars.DataAccess;
using Cars.Management;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Cars.ApiCommon.Extensions
{
    public static class StartupExtensions
    {
        /// <summary>
        /// Registers services in the DI container.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance.</param>
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;
            
            services.AddSingleton<ICarDataProvider, CarDataProvider>();
            services.AddSingleton<ICarManagementProvider, CarManagementProvider>();
        }

        /// <summary>
        /// Adds cosmos container options.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance.</param>
        public static void AddCosmosContainerOptions(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            ConfigurationManager configuration = builder.Configuration;
            IServiceCollection services = builder.Services;

            services.AddOptionsWithValidation<CosmosContainerOptions>(
                configuration.GetSection(CosmosContainerOptions.SectionKey));
        } 


        /// <summary>
        /// Add cosmos account options and initialize the Cosmos client.
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder instance.</param>
        public static void AddCosmosAccountOptions(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            ConfigurationManager configuration = builder.Configuration;
            IServiceCollection services = builder.Services;

            services.AddOptionsWithValidation<CosmosAccountOptions>(
                configuration.GetSection(CosmosAccountOptions.SectionKey))
                .Configure<ILoggerFactory, IOptions<CosmosContainerOptions>>((options, loggerFactory, cosmosContainerOptions) =>
                    {
                        ArgumentNullException.ThrowIfNull(loggerFactory, nameof(loggerFactory));
                        ArgumentNullException.ThrowIfNull(cosmosContainerOptions, nameof(cosmosContainerOptions));

                        var logger = loggerFactory.CreateLogger<CosmosAccountOptions>();
                        options.InitializeCosmosClient(logger, cosmosContainerOptions.Value);
                    }
                );
        }

        public static OptionsBuilder<TOptions> AddOptionsWithValidation<TOptions>(
            this IServiceCollection services,
            IConfigurationSection configurationSection)
            where TOptions : class
        {
            ArgumentNullException.ThrowIfNull(services, nameof(services));
            ArgumentNullException.ThrowIfNull(configurationSection, nameof(configurationSection));

            return services
                .AddOptions<TOptions>()
                .Bind(configurationSection)
                .ValidateDataAnnotations();
        }
    }
}
