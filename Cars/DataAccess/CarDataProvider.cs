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
        
        public async Task UpdateCarAsync(string id, CarUpdatePayload updatePayload)
        {
            try
            {
                // Create patch operations
                var patchOperations = CreatePatchOperations(updatePayload);
                
                // If no properties to update, return early
                if (!patchOperations.Any())
                {
                    logger.LogInformation($"No properties to update for car with ID: {id}");
                    return;
                }
                
                // Create a transactional batch with all patch operations
                var batch = cosmosClient.CreateTransactionalBatch(new PartitionKey(id));
                batch.PatchItem(id, [.. patchOperations]);
                
                // Execute the batch transaction
                var response = await batch.ExecuteAsync();
                
                // Check if the batch operation was successful
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogError($"Failed to update car with ID: {id}, Status: {response.StatusCode}");
                    throw new Exception($"Failed to update car with ID: {id}. Status: {response.StatusCode}, Message: {response.ErrorMessage}, FailedRequests: {response.Diagnostics.GetFailedRequestCount()}, Exception: {response.Diagnostics.GetQueryMetrics()}");
                }
                
                logger.LogInformation($"Successfully updated car with ID: {id}");
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                logger.LogError($"Car not found with ID: {id}");
                throw new DataNotFoundException($"Car with ID {id} not found");
            }
            catch (CosmosException e)
            {
                logger.LogError($"Failed to update car: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates a list of patch operations based on the properties in the update payload
        /// </summary>
        private static List<PatchOperation> CreatePatchOperations(CarUpdatePayload updatePayload)
        {
            var patchOperations = new List<PatchOperation>();
            
            // Add patch operations for each property that needs to be updated
            if (updatePayload.Make != null)
            {
                patchOperations.Add(PatchOperation.Set("/make", updatePayload.Make));
            }
            
            if (updatePayload.Model != null)
            {
                patchOperations.Add(PatchOperation.Set("/model", updatePayload.Model));
            }
            
            if (updatePayload.Year != null)
            {
                patchOperations.Add(PatchOperation.Set("/year", updatePayload.Year));
            }
            
            // For imageUrl, we need to handle it explicitly
            if (updatePayload.GetType().GetProperty("ImageUrl")?.GetValue(updatePayload) != null)
            {
                patchOperations.Add(PatchOperation.Set("/imageUrl", updatePayload.ImageUrl));
            }
            
            return patchOperations;
        }
    }
}