using api.Dtos.Playlist;
using api.Models;

namespace api.Mappers
{
    public static class PlaylistMapper
    {
        public static PlaylistDto ToDto(this Playlist playlist)
        {
            return new PlaylistDto
            {
                Id = playlist.Id,
                Name = playlist.Name,
                Songs = playlist.Songs
            };
        }

        public static Playlist ToPlaylistFromCreateDto(this CreatePlaylistRequestDto createPlaylistRequest)
        {
            return new Playlist
            {
                Name = createPlaylistRequest.Name,
                Songs = createPlaylistRequest.Songs
            };
        }
    }
}