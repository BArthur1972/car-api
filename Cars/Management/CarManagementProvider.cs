using Microsoft.Azure.Cosmos;
using Cars.ApiCommon.Models.Resources;
using Cars.ApiCommon.Models;
using Cars.DataAccess;
using Cars.ApiCommon.Extensions;

namespace Cars.Management
{
    public class CarManagementProvider : ICarManagementProvider
    {
        private readonly ICarDataProvider carDataProvider;
        private readonly ILogger<CarManagementProvider> logger;

        public CarManagementProvider(ICarDataProvider carDataProvider, ILogger<CarManagementProvider> logger)
        {
            this.carDataProvider = carDataProvider;
            this.logger = logger;
        }

        public async Task AddCar(CarRequestPayload carRequestPayload)
        {
            try
            {
                Car newCar = carRequestPayload.ToCar();
                await carDataProvider.AddCarAsync(newCar);
                logger.LogInformation("Added car: " + newCar.ToString());
            }
            catch (Exception e)
            {
                logger.LogError("Failed to add car: " + e.Message);
                throw;
            }
        }

        public async Task<IEnumerable<CarResponsePayload>> GetCars()
        {
            try
            {
                var response = await carDataProvider.GetCarsAsync();

                logger.LogInformation("Cars obtained: " + response.Count() + " cars");
                return response;
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get cars: " + e);
                throw;
            }
        }

        public async Task<CarResponsePayload?> GetCar(string id)
        {
            CarResponsePayload? car = null;
            try
            {
                var response = await carDataProvider.GetCarAsync(id);
                logger.LogInformation("Car obtained: " + response.ToString());
                car = response;
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get car: " + e);
                throw;
            }

            return car;
        }

        public async Task RemoveCar(string id)
        {
            try
            {
                await carDataProvider.RemoveCarAsync(id);
                logger.LogInformation("Removed car: " + id);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to remove car: " + e);
                throw;
            }
        }

        public async Task UpdateCar(string id, CarUpdatePayload updatePayload)
        {
            try
            {
                await carDataProvider.UpdateCarAsync(id, updatePayload);
                logger.LogInformation($"Updated car with ID: {id}, Changes: {updatePayload.ToString()}");
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to update car with ID: {id}: {e.Message}");
                throw;
            }
        }
    }
}
