using Microsoft.Azure.Cosmos;
using Cars.Cosmos;
using Cars.Cosmos.Options;
using Cars.ApiCommon.Models.Resources;
using Cars.ApiCommon.Models;
using Cars.ApiCommon.Exceptions;
using Microsoft.Extensions.Options;

namespace Cars.Providers
{
    public class CarProvider
    {
        private readonly CosmosAccountOptions cosmosAccountOptions;
        private readonly CosmosContainerOptions cosmosContainerOptions;
        private readonly Container container;
        private readonly ILogger<CarProvider> logger;

        public CarProvider(IOptions<CosmosOptions> cosmosOptions, ILogger<CarProvider> logger)
        {
            this.cosmosAccountOptions = cosmosOptions.Value.AccountOptions;
            this.cosmosContainerOptions = cosmosOptions.Value.ContainerOptions;
            this.logger = logger;
            CosmosConnection cosmosConnection = new CosmosConnection(cosmosAccountOptions, cosmosContainerOptions, this.logger);
            this.container = cosmosConnection.GetContainer();
        }

        public async Task AddCar(CarRequestPayload car)
        {
            try
            {
                Car newCar = new Car(car.Make, car.Model, car.ImageUrl);
                await container.CreateItemAsync<Car>(newCar);
                logger.LogInformation("Added car: " + newCar.ToString());
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to add car: " + e.Message);
            }
        }

        public async Task<IEnumerable<CarResponsePayload>> GetCars()
        {
            var cars = new List<CarResponsePayload>();
            try
            {
                var query = container.GetItemQueryIterator<CarResponsePayload>("SELECT * FROM c");
                var response = await query.ReadNextAsync();

                if (response.Count == 0)
                {
                    logger.LogError("No cars found");
                    throw new DataNotFoundException("No cars found");
                }

                logger.LogDebug("Cars obtained: " + response.Count + " cars");
                cars.AddRange(response);
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to get cars: " + e);
            }
            return cars;
        }

        public async Task<CarResponsePayload?> GetCar(string id)
        {
            try {
            QueryDefinition queryText = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", id);
            var query = container.GetItemQueryIterator<CarResponsePayload>(queryText);
            var response = await query.ReadNextAsync();

            Console.WriteLine("Car found");

            if (response.Count == 0)
            {
                logger.LogError("Car not found");
                throw new DataNotFoundException("Car not found");
            }
            return response.FirstOrDefault();
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to get car: " + e);
                return null;
            }
        }

        public async Task RemoveCar(string id)
        {
            try
            {
                await container.DeleteItemAsync<Car>(id, new PartitionKey(id));
                logger.LogInformation("Removed car with Id: " + id);
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to remove car: " + e);
            }
        }
    }
}
