using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Cars.ApiCommon.Models.Resources
{
    public class CarRequestPayload
    {
        [Required]
        [JsonPropertyName("make")]
        public string Make { get; set; }

        [Required]
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [Required]
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }
        
        public CarRequestPayload(string make, string model, int year, string? imageUrl = null)
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