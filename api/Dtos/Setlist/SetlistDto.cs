using System.Collections.Generic;

namespace api.Dtos.Setlist
{
    public class SetlistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        // This now holds the IDs of the selected songs, in order
        public List<int> SongIds { get; set; }
    }

    public class CreateSetlistRequestDto
    {
        public string Name { get; set; }
        public List<string> Songs { get; set; }
    }
}
