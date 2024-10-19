using Cars.Cosmos.Options;
using Microsoft.Azure.Cosmos;

namespace Cars.Cosmos
{
    public class CosmosConnection
    {
        public CosmosClient? cosmosClient;

        public readonly CosmosContainerOptions cosmosContainerOptions;

        public readonly ILogger logger;

        public CosmosConnection(CosmosContainerOptions cosmosContainerOptions, ILogger logger)
        {
            this.cosmosContainerOptions = cosmosContainerOptions;
            this.logger = logger;
        }

        public Container GetContainer()
        {
            cosmosClient = new CosmosClient("AccountEndpoint=[COSMOS_ACOUNT_ENDPOINT];AccountKey=[COSMOS_ACCOUNT_KEY];");
            
            logger.LogInformation($"Cosmos DB client initialized for endpoint: {this.cosmosClient.Endpoint.AbsoluteUri}");

            var container = this.cosmosClient.GetContainer(cosmosContainerOptions.DatabaseId, cosmosContainerOptions.ContainerId);

            logger.LogInformation($"Cosmos DB container initialized for database: {cosmosContainerOptions.DatabaseId} and container: {cosmosContainerOptions.ContainerId}");

            return container;
        }


    }
}
