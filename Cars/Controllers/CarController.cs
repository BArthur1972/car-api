using Microsoft.AspNetCore.Mvc;
using Cars.Providers;
using Cars.Models.Resources;

namespace Cars.Controllers
{
    [ApiController]
    [Route("cars")]
    public class CarController : ControllerBase
    {
        private readonly ILogger<CarController> logger;

        private readonly CarProvider carProvider;

        public CarController(ILogger<CarController> logger, CarProvider carProvider)
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
                logger.LogError("Failed to get cars", e);
                return StatusCode(500, "Internal Server Error. Failed to get cars. Check logs for more information.");
            }
        }

        [HttpGet("/getCar/{id}")]
        public async Task<ActionResult> GetCar(string id)
        {
            try
            {
                CarResponsePayload car = await carProvider.GetCar(id).ConfigureAwait(false);
                logger.LogInformation("Car obtained: " + car.ToString());
                return Ok(car);
            }
            catch (Exception e)
            {
                logger.LogError("Failed to get car", e);
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
                logger.LogError("Failed to add car", e);
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
                logger.LogError("Failed to remove car", e);
                return StatusCode(500, "Internal Server Error. Failed to remove car. Check logs for more information.");
            }
        }
    }
}
