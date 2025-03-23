namespace Cars.ApiCommon.Cosmos.Options
{
    public class CosmosContainerOptions
    {
        public const string SectionKey = "CosmosDB:CosmosContainerOptions";

        public string DatabaseId { get; set; } = null!;
        public string ContainerId { get; set; } = null!;
    }
}