namespace api.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string SongName { get; set; }
        public string Artist { get; set; }
        public int BPM { get; set; }
        public string Tone { get; set; }
        public string CoverImage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<AudioFile> AudioFiles { get; set; }
        public ICollection<SetlistSong> SetlistSongs { get; set; }
    }
}
