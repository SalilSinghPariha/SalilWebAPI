using System.ComponentModel.DataAnnotations;

namespace NZWalks.Models.DTO
{
    public class UpdateWalkRequestDto
    {
        [Required]
        [MaxLength(10, ErrorMessage = "Code should not exceed more than 10 chanracters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100, ErrorMessage = "Code should not exceed more than 100 chanracters")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, 50)]
        public double LengthInKm { get; set; }

        public string? WalkImangeUrl { get; set; }

        [Required]
        public Guid DifficultyId { get; set; }

        [Required]
        public Guid RegionId { get; set; }
    }
}
