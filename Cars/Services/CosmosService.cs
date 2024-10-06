using Microsoft.Azure.Cosmos;
using Cars.Models;
using Cars.Models.Resources;

namespace Cars.Services
{
    public class CosmosService
    {
        private readonly CosmosClient cosmosClient;
        private readonly Container container;
        private readonly ILogger<CosmosService> logger;

        public CosmosService(ILogger<CosmosService> logger)
        {
            // TODO: Use Azure Key Vault to store the Cosmos DB account key and endpoint and retrieve them from there
            cosmosClient = new CosmosClient("AccountEndpoint=[COSMOS_ACOUNT_ENDPOINT];AccountKey=[COSMOS_ACCOUNT_KEY];");
            container = cosmosClient.GetContainer("carsapp", "car");
            this.logger = logger;
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
