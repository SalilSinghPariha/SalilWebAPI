using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NZWalks.Models.DTO
{
    public class WalkDto
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public double LengthInKm { get; set; }

        public string? WalkImangeUrl { get; set; }

        // this is we are commenting sinc ewe don't need to expose and we can use include in repository while getting results
        //Otherwise we can use this so in order to use include in repository and here with property so it will fetch record nased on that
        //same we need to handle mapp as well 
        //public Guid DifficultyId { get; set; }

        //public Guid RegionId { get; set; }

        public RegionDto Region { get; set; }

        public DifficultyDto difficulty { get; set; }
    }
}
