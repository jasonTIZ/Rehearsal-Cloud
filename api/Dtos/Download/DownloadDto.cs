namespace api.Dtos.Download
{
    public class DownloadDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }
        public DateTime DownloadedAt { get; set; }
        public bool IsOfflineAvailable { get; set; }
    }
}