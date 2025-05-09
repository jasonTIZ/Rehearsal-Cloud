using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Customization
    {
        public int Id { get; set; }

        [Required]
        public int SongId { get; set; }
        public Song Song { get; set; } // Relación con Song

        [MaxLength(50)]
        public string VolumeLevel { get; set; }

        public string EqualizationSettings { get; set; }

        public bool LoopEnabled { get; set; }

        public bool FadeIn { get; set; }

        public bool FadeOut { get; set; }
    }
}