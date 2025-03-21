namespace Cars.ApiCommon.Cosmos.Options
{
    public class CosmosAccountOptions
    {
        public string AccountEndpoint { get; set; } = null!;

        public string AccountKey { get; set; } = null!;

        /// <summary>
        /// Determines which method to use for authentication.
        /// </summary>
        public bool UseMSICredentials { get; set; } = false;
    }
}