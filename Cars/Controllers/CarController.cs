using Microsoft.AspNetCore.Mvc;
using Cars.Management;
using Cars.ApiCommon.Models.Resources;
using Cars.ApiCommon.Exceptions;
namespace Cars.Controllers
{
    [ApiController]
    [Route("cars")]
    public class CarController : ControllerBase
    {
        private readonly ILogger<CarController> logger;

        private readonly CarManagementProvider carProvider;

        public CarController(ILogger<CarController> logger, CarManagementProvider carProvider)
        {
            this.logger = logger;
            this.carProvider = carProvider;
        }

        [HttpGet]
        [Route("/getCars")]
        public async Task<ActionResult> GetCars()
        {
            try
            {
                IEnumerable<CarResponsePayload> cars = await carProvider.GetCars().ConfigureAwait(false);
                logger.LogInformation("Cars obtained: " + cars.Count() + " cars");
                return Ok(cars);
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
                CarResponsePayload? car = await carProvider.GetCar(id).ConfigureAwait(false);
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
                return NotFound("Car not found." + e.Message);
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
                await carProvider.AddCar(car).ConfigureAwait(false);
                return Ok("Successfully added car: " + car.ToString());
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
                await carProvider.RemoveCar(id).ConfigureAwait(false);
                return Ok("Successfully removed car with id: " + id);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to remove car");
                return StatusCode(500, "Internal Server Error. Failed to remove car. Check logs for more information.");
            }
        }
    }
}
