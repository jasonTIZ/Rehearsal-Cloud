using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace api.Dtos.Setlist
{
    public class SetlistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<int> SetlistSongs { get; set; }
    }

    public class CreateSetlistRequestDto
    {
        [JsonPropertyOrder(1)]
        public string Name { get; set; }

        [JsonPropertyOrder(2)]
        public DateTime Date { get; set; }

        [JsonPropertyOrder(3)]
        public List<int> SetlistSongs { get; set; } = new();  // Default to empty list
    }
}
