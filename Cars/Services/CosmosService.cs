namespace Cars.Services
{
    using Microsoft.Azure.Cosmos;
    using Cars.Models;
    using Cars.Models.Resources;

    public class CosmosService
    {
        private readonly CosmosClient cosmosClient;
        private readonly Container container;
        private readonly ILogger<CosmosService> logger;

        public CosmosService(ILogger<CosmosService> logger)
        {
            // TODO: Use Azure Key Vault to store the Cosmos DB account key and endpoint and retrieve them here using the Azure Key Vault SDK
            cosmosClient = new CosmosClient("AccountEndpoint=[COSMOS_ACOUNT_ENDPOINT];AccountKey=[COSMOS_ACCOUNT_KEY];");
            container = cosmosClient.GetContainer("carsapp", "car");
            this.logger = logger;
        }

        public async Task AddCar(CarRequestPayload car)
        {
            try
            {
                Car newCar = new Car(car.Make, car.Model, car.ImageUrl);
                await container.CreateItemAsync<Car>(newCar, new PartitionKey(car.Make));
                logger.LogDebug("Added car: " + car.ToString());
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to add car: " + e);
            }
        }

        public async Task<IEnumerable<CarRequestPayload>> GetCars()
        {
            var cars = new List<CarRequestPayload>();
            try
            {
                var query = container.GetItemQueryIterator<CarRequestPayload>("SELECT * FROM c");
                var response = await query.ReadNextAsync();
                logger.LogDebug("Cars obtained: " + response.Count + "cars");
                cars.AddRange(response);
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to get cars: " + e);
            }
            return cars;
        }

        public async Task<Car> GetCar(int id)
        {
            try {
            var query = container.GetItemQueryIterator<Car>("SELECT * FROM c WHERE c.Id = " + id);
            var response = await query.ReadNextAsync();
            logger.LogDebug("Car obtained.");
            return response.FirstOrDefault();
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to get car: " + e);
                return null;
            }

        }

        public async Task RemoveCar(int id)
        {
            var car = await GetCar(id);
            try
            {
                await container.DeleteItemAsync<Car>(car.Id.ToString(), new PartitionKey(car.Make));
                logger.LogDebug("Removed car");
            }
            catch (CosmosException e)
            {
                logger.LogError("Failed to remove car: " + e);
            }

        }
    }
}
