using Cars.ApiCommon.Cosmos.Options;
using Microsoft.Azure.Cosmos;

namespace Cars.ApiCommon.Cosmos
{
    public class CosmosFacade(
        CosmosAccountOptions cosmosAccountOptions,
        CosmosContainerOptions cosmosContainerOptions,
         ILogger logger)
    {
        public readonly CosmosAccountOptions cosmosAccountOptions = cosmosAccountOptions;
        public readonly CosmosContainerOptions cosmosContainerOptions = cosmosContainerOptions;
        public readonly ILogger logger = logger;

        public Container GetContainer()
        {
            if (cosmosAccountOptions.CosmosClient == null)
            {
                throw new ArgumentException("Cosmos DB Client is not initialized.");
            }

            Container container = cosmosAccountOptions.CosmosClient.GetContainer(cosmosContainerOptions.DatabaseId, cosmosContainerOptions.ContainerId);
            logger.LogInformation($"Cosmos DB container initialized for database: {cosmosContainerOptions.DatabaseId} and container: {cosmosContainerOptions.ContainerId}");

            return container;
        }
    }
}