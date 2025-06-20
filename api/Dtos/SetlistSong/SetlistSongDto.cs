using api.Dtos.Song;

namespace api.Dtos.SetlistSong
{
    public class SetlistSongDto
    {
        public int SetlistId { get; set; }
        public int SongId { get; set; }
        public SongDto Song { get; set; }
    }
}