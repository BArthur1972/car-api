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
            try
            {
                await cosmosClient.UpsertItemAsync<Car>(car, new PartitionKey(car.Id));
                logger.LogInformation("Added car: " + car.ToString());
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {   
                logger.LogError($"Car causing error: Id={car.Id}, Make={car.Make}, Model={car.Model} Year={car.Year}");
                
                throw new ArgumentException($"The car is invalid: {ex.Message}", ex);
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to add car: " + e.Message);
                throw;
            }
        }

        public async Task<CarResponsePayload> GetCarAsync(string id)
        {
            try
            {
                ItemResponse<CarResponsePayload> response =
                    await cosmosClient.ReadItemAsync<CarResponsePayload>(id, new PartitionKey(id));
                
                logger.LogInformation("Car obtained: " + response.Resource.ToString());
                return response.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogError("Car not found");
                throw new DataNotFoundException("Car not found");
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to get car: " + e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<CarResponsePayload>> GetCarsAsync()
        {
            List<CarResponsePayload>? cars = [];
            try
            {
                var query = cosmosClient.GetItemQueryIterator<CarResponsePayload>("SELECT * FROM c");
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    cars.AddRange(response);
                }

                if (cars.Count == 0)
                {
                    logger.LogError("No cars found");
                    throw new DataNotFoundException("No cars found");
                }

                logger.LogDebug("Cars obtained: " + cars.Count + " cars");
                return cars;
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to get cars: " + e);
                throw;
            }
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