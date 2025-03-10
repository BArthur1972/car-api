using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Cars.ApiCommon.Models.Resources
{
    public class CarRequestPayload
    {
        [Required]
        [JsonProperty("make")]
        public string Make { get; set; }

        [Required]
        [JsonProperty("model")]
        public string Model { get; set; }

        [Required]
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("imageUrl")]
        public string? ImageUrl { get; set; }
        
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