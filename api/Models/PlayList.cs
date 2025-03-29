using System.Collections.Generic;

namespace api.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Songs { get; set; } = new List<string>();
    }
}