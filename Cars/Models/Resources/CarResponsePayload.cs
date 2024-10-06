using Newtonsoft.Json;

namespace Cars.Models.Resources
{
    // This class will be used to deserialize the response payload
    public record CarResponsePayload
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "make")]
        public string Make { get; set; }

        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "imageUrl")]
        public string? ImageUrl { get; set; }

        public CarResponsePayload(string id, string make, string model, string imageUrl)
        {
            Id = id;
            Make = make;
            Model = model;
            ImageUrl = imageUrl;
        }

        public override string ToString()
        {
            return $"Car with id: {Id}, is a/an {Make} {Model}.";
        }
    }
}