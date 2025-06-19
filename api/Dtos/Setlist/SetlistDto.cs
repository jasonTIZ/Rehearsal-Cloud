using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using api.Dtos.SetlistSong;
namespace api.Dtos.Setlist
{
    public class SetlistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<SetlistSongDto> SetlistSongs { get; set; }
    }
}
