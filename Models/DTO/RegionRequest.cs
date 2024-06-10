using System.ComponentModel.DataAnnotations;

namespace NZWalks.Models.DTO
{
    public class RegionRequest
    {
        [Required]
        [MinLength(3, ErrorMessage="Code has to be minimum 3 characters")]
        [MaxLength(5, ErrorMessage="Code should not exceed more than 5 chanracters")]
        public string Code { get; set; }

        [Required]
        [MaxLength(10,ErrorMessage ="Code has to be maximum 3 characters")]
        public string Name { get; set; } 

        //? this means nullable property
        public string? RegionImageurl { get; set; }
    }
}
