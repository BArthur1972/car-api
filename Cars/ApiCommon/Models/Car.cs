using Newtonsoft.Json;

namespace Cars.ApiCommon.Models
{
    public class Car
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("imageUrl")]
        public string? ImageUrl { get; set; }

        public Car(string make, string model, int year, string? imageUrl = null)
        {
            Id = Guid.NewGuid().ToString();
            Make = make;
            Model = model;
            Year = year;
            ImageUrl = imageUrl;
        }

        public override string ToString()
        {
            return $"Car with id: {Id}, is a {Year} {Make} {Model}.";
        }
    }
}
