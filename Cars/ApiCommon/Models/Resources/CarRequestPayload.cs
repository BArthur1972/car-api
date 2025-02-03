using System.Text.Json.Serialization;

namespace Cars.ApiCommon.Models.Resources
{
    // This class will be used to deserialize the request payload
    public record CarRequestPayload
    {
        [JsonPropertyName("make")]
        public string Make { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        public CarRequestPayload(string make, string model, string imageUrl)
        {
            Make = make;
            Model = model;
            ImageUrl = imageUrl;
        }

        public override string ToString()
        {
            return $"Make: {Make}, Model: {Model}";
        }
    }
}
