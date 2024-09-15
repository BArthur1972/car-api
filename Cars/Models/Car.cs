namespace Cars.Models
{
    public class Car
    {
        private static int nextId = 1;
        public int Id { get; private set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string? ImageUrl { get; set; } = null!;

        public Car(string make, string model, string? imageUrl)
        {
            Id = nextId++;
            Make = make;
            Model = model;
            ImageUrl = imageUrl;
        }

        public Car(string make, string model)
        {
            Id = nextId++;
            Make = make;
            Model = model;
        }

        public override string ToString()
        {
            return $"Car with Id {Id} is a {Make} {Model}.";
        }
    }
}
