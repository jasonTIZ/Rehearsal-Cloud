using api.Dtos.CloudStorage;
using api.Models;

namespace api.Mappers
{
    public static class CloudStorageMapper
    {
        public static CloudStorageDto ToDto(this CloudStorage cloudStorage)
        {
            return new CloudStorageDto
            {
                Id = cloudStorage.Id,
                UserId = cloudStorage.UserId,
                FileName = cloudStorage.FileName,
                FileUrl = cloudStorage.FileUrl,
                UploadedAt = cloudStorage.UploadedAt,
                FileSize = cloudStorage.FileSize
            };
        }

        public static CloudStorage ToCloudStorageFromCreateDto(this CreateCloudStorageRequestDto request)
        {
            return new CloudStorage
            {
                UserId = request.UserId,
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                FileSize = request.FileSize
            };
        }
    }
}