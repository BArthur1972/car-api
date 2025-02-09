using Cars.ApiCommon.Cosmos;
using Cars.ApiCommon.Cosmos.Options;
using Cars.ApiCommon.Exceptions;
using Cars.ApiCommon.Models;
using Cars.ApiCommon.Models.Resources;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Cars.DataAccess
{
    public class CarDataProvider : ICarDataProvider
    {
        private readonly CosmosAccountOptions cosmosAccountOptions;
        private readonly CosmosContainerOptions cosmosContainerOptions;
        private readonly Container cosmosClient;
        private readonly ILogger<CarDataProvider> logger;
        
        public CarDataProvider(IOptions<CosmosOptions> cosmosOptions, ILogger<CarDataProvider> logger)
        {
            this.cosmosAccountOptions = cosmosOptions.Value.AccountOptions;
            this.cosmosContainerOptions = cosmosOptions.Value.ContainerOptions;
            this.logger = logger;
            CosmosConnection cosmosConnection = new CosmosConnection(cosmosAccountOptions, cosmosContainerOptions, this.logger);
            this.cosmosClient = cosmosConnection.GetContainer();
        }

        public async Task AddCarAsync(Car car)
        {
            await cosmosClient.UpsertItemAsync<Car>(car);
            logger.LogInformation("Added car: " + car.ToString());
        }

        public async Task<CarResponsePayload> GetCarAsync(string id)
        {
            try
            {
                QueryDefinition queryText = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", id);
                var query = cosmosClient.GetItemQueryIterator<CarResponsePayload>(queryText);
                var response = await query.ReadNextAsync();

                if (response.Count == 0)
                {
                    logger.LogError("Car not found");
                    throw new DataNotFoundException("Car not found");
                }

                logger.LogInformation("Car obtained: " + response.First().ToString());
                return response.First();
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to get car: " + e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<CarResponsePayload>> GetCarsAsync()
        {
            IEnumerable<CarResponsePayload>? cars = null;
            try
            {
                var query = cosmosClient.GetItemQueryIterator<CarResponsePayload>("SELECT * FROM c");
                var response = await query.ReadNextAsync();

                if (response.Count == 0)
                {
                    logger.LogError("No cars found");
                    throw new DataNotFoundException("No cars found");
                }

                logger.LogDebug("Cars obtained: " + response.Count + " cars");
                cars = response.ToList();
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to get cars: " + e);
                throw;
            }

            return cars;
        }

        public async Task RemoveCarAsync(string id)
        {
            try
            {
                await cosmosClient.DeleteItemAsync<Car>(id, new PartitionKey(id));
                logger.LogInformation("Deleted car with ID: " + id);
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to delete car: " + e.Message);
                throw;
            }
        }
    }
}