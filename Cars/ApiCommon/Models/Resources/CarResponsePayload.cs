using System.Text.Json.Serialization;

namespace Cars.ApiCommon.Models.Resources
{
    // This class will be used to deserialize the response payload
    public class CarResponsePayload
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("make")]
        public string Make { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        public CarResponsePayload(string id, string make, string model, int year, string? imageUrl = null)
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