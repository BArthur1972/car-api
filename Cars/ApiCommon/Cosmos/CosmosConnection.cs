using Azure.Core;
using Azure.Identity;
using Cars.ApiCommon.Cosmos.Options;
using Microsoft.Azure.Cosmos;

namespace Cars.ApiCommon.Cosmos
{
    public class CosmosConnection
    {
        public CosmosClient? cosmosClient;
        public readonly CosmosAccountOptions cosmosAccountOptions;
        public readonly CosmosContainerOptions cosmosContainerOptions;
        public readonly ILogger logger;

        public CosmosConnection(CosmosAccountOptions cosmosAccountOptions, CosmosContainerOptions cosmosContainerOptions, ILogger logger)
        {
            this.cosmosAccountOptions = cosmosAccountOptions;
            this.cosmosContainerOptions = cosmosContainerOptions;
            this.logger = logger;
        }

        public Container GetContainer()
        {
            if (cosmosAccountOptions.UseMSICredentials)
            {
                // Use DefaultAzureCredential for token-based auth
                TokenCredential tokenCredential = new DefaultAzureCredential();
                cosmosClient = new CosmosClient(cosmosAccountOptions.AccountEndpoint, tokenCredential);
                logger.LogInformation($"Cosmos DB client initialized with TokenCredential for endpoint: {cosmosClient.Endpoint.AbsoluteUri}");
            }
            else
            {
                cosmosClient = new CosmosClient($"AccountEndpoint={cosmosAccountOptions.AccountEndpoint};AccountKey={cosmosAccountOptions.AccountKey};");
                logger.LogInformation($"Cosmos DB client initialized with AccountKey for endpoint: {cosmosClient.Endpoint.AbsoluteUri}");
            }

            Container container;
            try
            {
                container = cosmosClient.GetContainer(cosmosContainerOptions.DatabaseId, cosmosContainerOptions.ContainerId);
                logger.LogInformation($"Cosmos DB container initialized for database: {cosmosContainerOptions.DatabaseId} and container: {cosmosContainerOptions.ContainerId}");
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to initialize Cosmos DB container: {e.Message}");
                throw;
            }

            return container;
        }
    }
}