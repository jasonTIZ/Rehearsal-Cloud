using api.Dtos.Setlist;
using api.Models;

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
                Songs = setlist.Songs
                    .Select(song => song.SongName) // Convertimos ICollection<Song> a List<string>
                    .ToList()
            };
        }

        public static Setlist ToEntity(SetlistDto setlistDto)
        {
            return new Setlist
            {
                Id = setlistDto.Id,
                Name = setlistDto.Name,
                Songs = setlistDto.Songs
                    .Select(songName => new Song { SongName = songName }) // Convertimos List<string> a ICollection<Song>
                    .ToList()
            };
        }
    }
}
