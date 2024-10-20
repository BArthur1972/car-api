using Cars.Cosmos.Options;
using Microsoft.Azure.Cosmos;

namespace Cars.Cosmos
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
            cosmosClient = new CosmosClient($"AccountEndpoint={cosmosAccountOptions.AccountEndpoint};AccountKey={cosmosAccountOptions.AccountKey};");
            
            logger.LogInformation($"Cosmos DB client initialized for endpoint: {this.cosmosClient.Endpoint.AbsoluteUri}");

            var container = this.cosmosClient.GetContainer(cosmosContainerOptions.DatabaseId, cosmosContainerOptions.ContainerId);

            logger.LogInformation($"Cosmos DB container initialized for database: {cosmosContainerOptions.DatabaseId} and container: {cosmosContainerOptions.ContainerId}");

            return container;
        }


    }
}
