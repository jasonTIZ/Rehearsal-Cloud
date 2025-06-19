using api.Dtos.AudioFile;

namespace api.Dtos.Song
{
    public class SongDto
    {
        public int Id { get; set; }
        public string SongName { get; set; }
        public string Artist { get; set; }
        public int BPM { get; set; }
        public string Tone { get; set; }
        public string CoverImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AudioFileDto> AudioFiles { get; set; }
    }
}