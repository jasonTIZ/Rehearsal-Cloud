
namespace api.Dtos.Song
{
    public class UpdateSongRequestDto
    {
        public string SongName { get; set; }
        public string Artist { get; set; }
        public int BPM { get; set; }
        public string Tone { get; set; }
        public IFormFile? CoverImage { get; set; }
        public IFormFile? ZipFile { get; set; }
    }
}