using System.ComponentModel.DataAnnotations;
namespace api.Dtos.Customization
{
    public class DeleteCustomizationRequestDto
    {
        [Required]
        public int Id { get; set; }
    }
}