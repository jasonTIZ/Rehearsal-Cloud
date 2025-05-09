using System.ComponentModel.DataAnnotations;
namespace api.Dtos.Download
{
    public class DeleteDownloadRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}