using api.Dtos.AudioFile;
using api.Dtos.Song;
using api.Models;

namespace api.Mappers
{
    public static class SongMapper
    {
        public static SongDto ToLightweightDto(this Song song)
        {
            return new SongDto
            {
                Id = song.Id,
                SongName = song.SongName,
                Artist = song.Artist,
                BPM = song.BPM,
                Tone = song.Tone,
                CoverImage = song.CoverImage,
                CreatedAt = song.CreatedAt,
                AudioFiles = null
            };
        }

        public static SongDto ToDto(this Song song)
        {
            return new SongDto
            {
                Id = song.Id,
                SongName = song.SongName,
                Artist = song.Artist,
                BPM = song.BPM,
                Tone = song.Tone,
                CoverImage = song.CoverImage,
                CreatedAt = song.CreatedAt,
                AudioFiles = song.AudioFiles?.Select(af => new AudioFileDto
                {
                    Id = af.Id,
                    FileName = af.FileName,
                    FileExtension = af.FileExtension,
                    FileSize = af.FileSize,
                    SongId = af.SongId,
                    FileUrl = $"/api/Song/{song.Id}/audio/{af.Id}"
                }).ToList()
            };
        }
    }
}