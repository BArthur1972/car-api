using Newtonsoft.Json;

namespace Cars.DataAccess.Entities.Resources
{
    // This class will be used to deserialize the response payload
    public class CarResponsePayload
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

        public CarResponsePayload(string id, string make, string model, int year, string? imageUrl)
        {
            Id = id;
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