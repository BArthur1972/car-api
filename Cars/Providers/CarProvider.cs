using Microsoft.Azure.Cosmos;
using Cars.Models;
using Cars.Models.Resources;
using Cars.Cosmos;
using Cars.Cosmos.Options;

namespace Cars.Providers
{
    public class CarProvider
    {
        private readonly Container container;
        private readonly ILogger<CarProvider> logger;

        public CarProvider(ILogger<CarProvider> logger)
        {
            this.logger = logger;
            CosmosConnection cosmosConnection = new CosmosConnection(new CosmosAccountOptions(), new CosmosContainerOptions(), this.logger);
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
                logger.LogDebug("Cars obtained: " + response.Count + " cars");
                cars.AddRange(response);
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to get cars: " + e);
            }
            return cars;
        }

        public async Task<CarResponsePayload> GetCar(string id)
        {
            try {
            QueryDefinition queryText = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", id);
            var query = container.GetItemQueryIterator<CarResponsePayload>(queryText);
            var response = await query.ReadNextAsync();

            Console.WriteLine("Car found");
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
