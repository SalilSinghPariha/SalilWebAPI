namespace NZWalks.Models.Domain
{
    public class Region
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        //? this means nullable property
        public string? RegionImageurl { get; set; }
    }
}
