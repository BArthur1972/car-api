using Cars.Models;
using Microsoft.AspNetCore.Mvc;
using Cars.Services;
using Cars.Models.Resources;

namespace Cars.Controllers
{
    [ApiController]
    [Route("car")]
    public class CarController : ControllerBase
    {
        private readonly ILogger<CarController> logger;
        private readonly CarService carService;
        private readonly CosmosService cosmosService;

        public CarController(ILogger<CarController> logger, CarService carService, CosmosService cosmosService)
        {
            this.logger = logger;
            this.carService = carService;
            this.cosmosService = cosmosService;
        }

        [HttpGet]
        [Route("/getCars")]
        public Task<IEnumerable<CarRequestPayload>> Get()
        {
            return cosmosService.GetCars();
        }

        [HttpGet("/getCar/{id}")]
        public Task<ActionResult<Car>> GetCar(int id)
        {
            var car = cosmosService.GetCar(id);
            if (car == null)
            {
                return Task.FromResult<ActionResult<Car>>(NotFound());
            }
            return Task.FromResult<ActionResult<Car>>(Ok(car));
        }

        [HttpPost]
        [Route("/addCar")]
        public async void Post([FromBody] CarRequestPayload car)
        {
            try {
                await cosmosService.AddCar(car);
            } catch (Exception e) {
                logger.LogError("Failed to add car", e);
            }
        }

        [HttpDelete("/removeCar/{id}")]
        public async void Delete(int id)
        {
            await cosmosService.RemoveCar(id);
        }
    }
}
