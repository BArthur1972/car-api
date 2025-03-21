using Newtonsoft.Json;

namespace Cars.ApiCommon.Models.Resources
{
    /// <summary>
    /// Payload model for partial updates to a car
    /// </summary>
    public class CarUpdatePayload
    {
        [JsonProperty("make")]
        public string? Make { get; set; }

        [JsonProperty("model")]
        public string? Model { get; set; }

        [JsonProperty("year")]
        public int? Year { get; set; }

        [JsonProperty("imageUrl")]
        public string? ImageUrl { get; set; }
        
        public CarUpdatePayload(string? make = null, string? model = null, int? year = null, string? imageUrl = null)
        {
            Make = make;
            Model = model;
            Year = year;     
            ImageUrl = imageUrl;
        }

        public override string ToString()
        {
            var props = new List<string>();
            if (Make != null) props.Add($"Make={Make}");
            if (Model != null) props.Add($"Model={Model}");
            if (Year != null) props.Add($"Year={Year}");
            if (ImageUrl != null) props.Add($"ImageUrl={ImageUrl}");
            
            return $"CarUpdate with changes to: {string.Join(", ", props)}";
        }
    }
}