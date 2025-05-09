using System.ComponentModel.DataAnnotations;
namespace api.Dtos.CloudStorage

{
    public class DeleteCloudStorageRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}