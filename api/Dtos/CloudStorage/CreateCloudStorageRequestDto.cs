using System.ComponentModel.DataAnnotations;
namespace api.Dtos.CloudStorage
{
    public class CreateCloudStorageRequestDto
    {
        [Required]
        public int UserId { get; set; }

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