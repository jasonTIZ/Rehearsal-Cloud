namespace api.Models
{
    public class AudioFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public long FileSize { get; set; }
        public int SongId { get; set; }
        public Song Song { get; set; }
    }
}
