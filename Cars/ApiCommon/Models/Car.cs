using System.Text.Json;
using Newtonsoft.Json;

namespace Cars.ApiCommon.Models
{
    public class Car
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "make")]
        public string Make { get; set; }

        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "imageUrl")]
        public string? ImageUrl { get; set; }


        [JsonConstructor]
        public Car(string make, string model, string? imageUrl)
        {
            Id = Guid.NewGuid().ToString();
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
