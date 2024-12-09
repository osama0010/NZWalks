using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class UpdateRegionDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Code has to be Minimum of 3 Char")]
        [MaxLength(3, ErrorMessage = "Code has to be Maximum of 3 Char")]
        public string Code { get; set; }
        [MaxLength(100, ErrorMessage = "Name can to be of Maximum 100 Char")]
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
