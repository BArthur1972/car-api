using Cars.Cosmos.Options;
using Cars.DataAccess;
using Cars.Management;
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

            services.AddSingleton<ICarDataProvider, CarDataProvider>();
            services.AddSingleton<ICarManagementProvider, CarManagementProvider>();
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
