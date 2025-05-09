using System.ComponentModel.DataAnnotations;

namespace api.Dtos.CloudStorage
{
    public class UpdateCloudStorageRequestDto
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileUrl { get; set; }

        [Required]
        public long FileSize { get; set; }
    }
}