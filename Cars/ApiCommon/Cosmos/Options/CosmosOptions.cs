namespace Cars.ApiCommon.Cosmos.Options
{
    public class CosmosOptions
    {
        public static string SectionKey => "CosmosOptions";

        public CosmosAccountOptions AccountOptions { get; set; } = null!;
        public CosmosContainerOptions ContainerOptions { get; set; } = null!;
    }
}