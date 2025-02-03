using Cars.Cosmos.Options;
using Cars.Providers;
using Microsoft.Extensions.Options;

namespace Cars
{
    public static class StartupExtensions
    {
        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            IServiceCollection services = builder.Services;
            ConfigurationManager configuration = builder.Configuration;

            services.AddOptionsWithValidation<CosmosOptions>(configuration.GetSection(CosmosOptions.SectionKey));

            Console.WriteLine("Registering services...");
            Console.WriteLine("Account Endpoint: " + configuration.GetSection(CosmosOptions.SectionKey).Get<CosmosOptions>()?.AccountOptions.AccountEndpoint.ToString());
            Console.WriteLine("Account Key: " + configuration.GetSection(CosmosOptions.SectionKey).Get<CosmosOptions>()?.AccountOptions.AccountKey.ToString());
            Console.WriteLine("Database ID: " + configuration.GetSection(CosmosOptions.SectionKey).Get<CosmosOptions>()?.ContainerOptions.DatabaseId.ToString());
            Console.WriteLine("Container ID: " + configuration.GetSection(CosmosOptions.SectionKey).Get<CosmosOptions>()?.ContainerOptions.ContainerId.ToString());

            services.AddSingleton<CarProvider>();
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
