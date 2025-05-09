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
                Date = setlist.Date,
                SongIds = setlist.SetlistSongs?
            .OrderBy(ss => ss.Order)
            .Select(static ss => ss.SongId)
            .ToList()
            };
        }

        public static Setlist ToEntity(SetlistDto dto)
        {
            return new Setlist
            {
                Id = dto.Id,
                Name = dto.Name,
                Date = dto.Date,
                SetlistSongs = dto.SongIds
                    .Select((songId, index) => new SetlistSong
                    {
                        SongId = songId,
                        Order = index
                    })
                    .ToList()
            };
        }
    }
}
