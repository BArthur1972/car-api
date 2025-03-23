using Microsoft.AspNetCore.Mvc;
using Cars.Management;
using Cars.DataAccess.Entities.Resources;
using Cars.ApiCommon.Exceptions;
using Cars.DataAccess.Entities;
namespace Cars.Controllers
{
    [ApiController]
    [Route("cars")]
    public class CarController(ILogger<CarController> logger, ICarManagementProvider carProvider) : ControllerBase
    {
        private readonly ILogger<CarController> logger = logger;
        private readonly ICarManagementProvider carManagementProvider = carProvider;

        [HttpGet]
        [Route("/getCars")]
        public async Task<ActionResult> GetCars()
        {
            try
            {
                IEnumerable<CarResponsePayload> cars = await carManagementProvider.GetCars().ConfigureAwait(false);
                logger.LogInformation("Cars obtained: " + cars.Count() + " cars");
                return Ok(cars);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to get cars");
                return StatusCode(500, "Internal Server Error. Failed to get cars. Check logs for more information.");
            }
        }

        [HttpGet("/getCar/{id}")]
        public async Task<ActionResult> GetCar(string id)
        {
            try
            {
                CarResponsePayload? car = await carManagementProvider.GetCar(id).ConfigureAwait(false);
                if (car == null)
                {
                    logger.LogError("Car with id: " + id + " not found.");
                    throw new DataNotFoundException("Car not found");
                }
                logger.LogInformation("Car obtained: " + car.ToString());
                return Ok(car);
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to get car");
                return StatusCode(500, "Internal Server Error. Failed to get car. Check logs for more information.");
            }
        }

        [HttpPost]
        [Route("/addCar")]
        public async Task<ActionResult> AddCar([FromBody] CarRequestPayload car)
        {
            try {
                Car newCar = await carManagementProvider.AddCar(car).ConfigureAwait(false);
                return Ok("Successfully added car: " + newCar.ToString());
            } catch (Exception e) {
                logger.LogError(e, "Failed to add car");
                return StatusCode(500, "Internal Server Error. Failed to add car. Check logs for more information.");
            }
        }

        [HttpDelete("/removeCar/{id}")]
        public async Task<ActionResult> DeleteCar(string id)
        {
            try
            {
                await carManagementProvider.RemoveCar(id).ConfigureAwait(false);
                return Ok("Successfully removed car with id: " + id);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to remove car");
                return StatusCode(500, "Internal Server Error. Failed to remove car. Check logs for more information.");
            }
        }

        [HttpPatch("/updateCar/{id}")]
        public async Task<ActionResult> UpdateCar(string id, [FromBody] CarUpdatePayload updatePayload)
        {
            try
            {
                // Validate that at least one property is being updated
                var properties = typeof(CarUpdatePayload).GetProperties();
                bool hasUpdates = properties.Any(p => p.GetValue(updatePayload) != null);
                
                if (!hasUpdates)
                {
                    return BadRequest("Update payload must contain at least one property to update");
                }
                
                CarResponsePayload updatedCar = await carManagementProvider.UpdateCar(id, updatePayload).ConfigureAwait(false);
                return Ok($"Successfully updated car: " + updatedCar.ToString());
            }
            catch (DataNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Failed to update car with id: {id}");
                return StatusCode(500, "Internal Server Error. Failed to update car. Check logs for more information.");
            }
        }
    }
}
