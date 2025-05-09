using System;
using System.Collections.Generic;

namespace api.Models
{
    public class Setlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        // Many-to-many with Songs
        public ICollection<SetlistSong> SetlistSongs { get; set; }
    }
}
