using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace Cars.ApiCommon.Cosmos.Options
{
    public class CosmosAccountOptions
    {
        public const string SectionKey = "CosmosDB:CosmosAccountOptions";

        public string AccountEndpoint { get; set; } = null!;

        public string? AccountKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether MSI credentials will be used or not.
        /// </summary>
        public bool UseMSICredentials { get; set; }

        /// <summary>
        /// Gets or sets the Cosmos client to use for connecting to the Cosmos DB account.
        /// </summary>
        public CosmosClient? CosmosClient { get; private set; }

        /// <summary>
        /// Initializes the Cosmos client with the specified options.
        /// </summary>
        /// <param name="logger">The logger to use for logging.</param>
        /// <param name="cosmosContainerOptions">The Cosmos container options.</param>
        public void InitializeCosmosClient(ILogger logger, CosmosContainerOptions? cosmosContainerOptions = null)
            {
                ArgumentNullException.ThrowIfNull(cosmosContainerOptions, nameof(cosmosContainerOptions));

                if (UseMSICredentials)
                {
                    var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID");
                    var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID");
                    var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET");
                    logger.LogDebug($"Using MSI credentials with clientId: {clientId}, tenantId: {tenantId}, clientSecret: {clientSecret}");

                    IReadOnlyList<(string, string)> container =[ (cosmosContainerOptions.DatabaseId, cosmosContainerOptions.ContainerId) ];
                    
                    TokenCredential tokenCredential = new DefaultAzureCredential();
                    // Ensure the CosmosCLient is initialized synchronously when registering as a singleton.
                    CosmosClient ??= CosmosClient.CreateAndInitializeAsync(AccountEndpoint, tokenCredential, container).GetAwaiter().GetResult();
                    logger.LogInformation($"Cosmos DB client initialized with TokenCredential for endpoint: {CosmosClient.Endpoint.AbsoluteUri}");
                }
                else
                {
                    CosmosClient ??= new CosmosClient($"AccountEndpoint={AccountEndpoint};AccountKey={AccountKey};");
                    logger.LogInformation($"Cosmos DB client initialized with AccountKey for endpoint: {CosmosClient.Endpoint.AbsoluteUri}");
                }
            }
    }
}