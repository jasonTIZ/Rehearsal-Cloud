namespace api.Dtos.AudioFile
{
    public class AudioFileDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public long FileSize { get; set; }
        public int SongId { get; set; }
        public string FileUrl { get; set; }
    }
}