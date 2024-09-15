using System.Text.Json.Serialization;

namespace Cars.Models.Resources
{
    // This class will be used to deserialize the request payload
    public record CarRequestPayload
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string ImageUrl { get; set; }

        public CarRequestPayload(string make, string model, string imageUrl)
        {
            Make = make;
            Model = model;
            ImageUrl = imageUrl;
        }

        public override string ToString()
        {
            return $"Make: {Make}, Model: {Model}, ImageUrl: {ImageUrl}";
        }
    }
}
