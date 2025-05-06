namespace api.Dtos.CloudStorage
{
    public class CloudStorageDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        public long FileSize { get; set; }
    }
}