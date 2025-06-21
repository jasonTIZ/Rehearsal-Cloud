using api.Models;
using api.Dtos.Setlist;
using api.Dtos.SetlistSong;
using api.Dtos.Song;
using api.Dtos.AudioFile;

namespace api.Mappers
{
    public static class SetlistMapper
    {
        public static SetlistDto ToDto(this Setlist setlist)
        {
            return new SetlistDto
            {
                Id = setlist.Id,
                Name = setlist.Name,
                Date = setlist.Date,
                SetlistSongs = setlist.SetlistSongs?.Select(ss => new SetlistSongDto
                {
                    SetlistId = ss.SetlistId,
                    SongId = ss.SongId,
                    Song = ss.Song != null ? new SongDto
                    {
                        Id = ss.Song.Id,
                        SongName = ss.Song.SongName,
                        Artist = ss.Song.Artist,
                        BPM = ss.Song.BPM,
                        Tone = ss.Song.Tone,
                        CoverImage = ss.Song.CoverImage,
                        CreatedAt = ss.Song.CreatedAt,
                        AudioFiles = ss.Song.AudioFiles?.Select(af => new AudioFileDto
                        {
                            Id = af.Id,
                            FileName = af.FileName,
                            FileExtension = af.FileExtension,
                            FileSize = af.FileSize,
                            SongId = af.SongId,
                            FileUrl = af.Id > 0 ? $"/api/Song/{ss.Song.Id}/audio/{af.Id}" : null // Ensure valid ID
                        }).Where(af => af.FileUrl != null).ToList() // Filter out null FileUrl
                    } : null
                }).ToList()
            };
        }
        public static SetlistWithoutSongsDto ToWithoutSongsDto(this Setlist setlist)
        {
            return new SetlistWithoutSongsDto
            {
                Id = setlist.Id,
                Name = setlist.Name,
                Date = setlist.Date
            };
        }
    }
}
