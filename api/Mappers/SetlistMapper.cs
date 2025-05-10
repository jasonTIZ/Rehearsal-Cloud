using api.Models;
using api.Dtos.Setlist; 
using System.Linq;

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
                SetlistSongs = setlist.SetlistSongs?
                    .OrderBy(ss => ss.Order)
                    .Select(ss => ss.SongId)
                    .ToList()
            };
        }
    }
}
