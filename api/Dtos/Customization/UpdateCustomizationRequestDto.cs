using System.ComponentModel.DataAnnotations;
namespace api.Dtos.Customization
{
    public class UpdateCustomizationRequestDto
    {
        [Required]
        public int SongId { get; set; }

        [MaxLength(50)]
        public string VolumeLevel { get; set; }

        public string EqualizationSettings { get; set; }

        public bool LoopEnabled { get; set; }

        public bool FadeIn { get; set; }

        public bool FadeOut { get; set; }
    }
}