
using System.Text.Json.Serialization;
namespace api.Dtos.Setlist
{
    public class CreateSetlistRequestDto
    {
        [JsonPropertyOrder(1)]
        public string Name { get; set; }

        [JsonPropertyOrder(2)]
        public DateTime Date { get; set; }
    }
}