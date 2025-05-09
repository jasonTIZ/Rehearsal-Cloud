using System.Collections.Generic;

namespace api.Dtos.Setlist
{
    public class SetlistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Songs { get; set; }
    }

    public class CreateSetlistRequestDto
    {
        public string Name { get; set; }
        public List<string> Songs { get; set; }
    }
}
