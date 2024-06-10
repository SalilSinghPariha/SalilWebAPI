namespace NZWalks.Models.Domain
{
    public class Walk
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; }= string.Empty;

        public double LengthInKm { get; set; }

        public string? WalkImangeUrl { get; set; }

        public Guid DifficultyId { get; set; }

        public Guid RegionId { get; set; }

        //Navigation property which is one to one
        public Difficulty? Difficulty { get; set; }

        public Region?  Region { get; set; }
    }
}
