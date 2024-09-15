using Cars.Models;
using Cars.Models.Resources;

namespace Cars.Services
{
    public class CarService
    {
        List<Car> cars = [
            new Car("Toyota", "Corolla", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-corolla-2020-%20%281%29.jpg"),
                new Car("Toyota", "Camry", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-camry-2020-%20%281%29.jpg"),
                new Car("Toyota", "Prius", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-prius-2020-%20%281%29.jpg"),
                new Car("Toyota", "RAV4", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-rav4-2020-%20%281%29.jpg"),
                new Car("Toyota", "Highlander", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-highlander-2020-%20%281%29.jpg"),
                new Car("Toyota", "4Runner", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-4runner-2020-%20%281%29.jpg"),
                new Car("Toyota", "Tacoma", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-tacoma-2020-%20%281%29.jpg"),
                new Car("Toyota", "Tundra", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-tundra-2020-%20%281%29.jpg"),
                new Car("Toyota", "Supra", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-supra-2020-%20%281%29.jpg"),
                new Car("Toyota", "Land Cruiser", "https://www.carsguide.com.au/sites/default/files/styles/editorial_hero/public/2020-08/toyota-landcruiser-2020-%20%281%29.jpg"),
            ];

        private readonly ILogger<CarService> logger;

        public CarService(ILogger<CarService> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<Car> GetCars()
        {
            if (cars.Count == 0)
            {
                logger.LogError("No cars found");
            }
            logger.Log(LogLevel.Information, "Getting all cars");
            return cars;
        }

        public Car? GetCar(int id)
        {
            var car = cars.Find(car => car.Id == id);

            if (car == null)
            {
                logger.LogError("Car with Id {id} not found", id);
            }
            else
            {
                logger.Log(LogLevel.Information, "Getting car with Id {id}", id);
            }

            return car;
        }

        public void AddCar(CarRequestPayload car)
        {
            // Add car to the list
            cars.Add(new Car(car.Make, car.Model, car.ImageUrl));
            this.logger.Log(LogLevel.Information, message: "Added, {car}", car);
        }

        public void RemoveCar(int id)
        {
            try
            {
                var car = GetCar(id);
                if (car != null)
                {
                    cars.Remove(car);
                    logger.Log(LogLevel.Information, "Removed car with Id {id}", id);
                    return;
                }
                else
                {
                    logger.LogWarning("Car with Id {id} not found", id);
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to remove car with Id, {id}", e);
            }
        }
    }
}
