using api.Dtos.Download;
using api.Models;

namespace api.Mappers
{
    public static class DownloadMapper
    {
        public static DownloadDto ToDto(this Download download)
        {
            return new DownloadDto
            {
                Id = download.Id,
                UserId = download.UserId,
                SongId = download.SongId,
                DownloadedAt = download.DownloadedAt,
                IsOfflineAvailable = download.IsOfflineAvailable
            };
        }

        public static Download ToDownloadFromCreateDto(this CreateDownloadRequestDto request)
        {
            return new Download
            {
                UserId = request.UserId,
                SongId = request.SongId,
                IsOfflineAvailable = request.IsOfflineAvailable
            };
        }
    }
}