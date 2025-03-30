using System.Text.Json.Serialization;

namespace Cars.DataAccess.Entities.Resources
{
    public class CarRequestPayload
    {
        public required string Make { get; set; }

        public required string Model { get; set; }

        public required int Year { get; set; }

        public string? ImageUrl { get; set; }
        
        [JsonConstructor]
        public CarRequestPayload(string make, string model, int year, string? imageUrl)
        {
            Make = make;
            Model = model;
            Year = year;     
            ImageUrl = imageUrl;
        }

        public override string ToString()
        {
            return $"Car is a {Year} {Make} {Model}.";
        }
    }
}