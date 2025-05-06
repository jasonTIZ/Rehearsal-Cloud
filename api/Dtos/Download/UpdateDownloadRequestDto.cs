using System.ComponentModel.DataAnnotations;
namespace api.Dtos.Download
{
    public class UpdateDownloadRequestDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SongId { get; set; }

        public bool IsOfflineAvailable { get; set; }
    }
}