using api.Dtos.Song;
using api.Models;

namespace api.Mappers
{
    public static class SongMapper
    {

        // Mapea de Song a un DTO de respuesta para obtener los datos de una canción
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
                CreatedAt = song.CreatedAt
            };
        }

        // Mapea de CreateSongRequestDto a Song (cuando se crea una nueva canción)
        public static Song ToSongFromCreateDto(this CreateSongRequestDto createSongRequest)
        {
            return new Song
            {
                SongName = createSongRequest.SongName,
                Artist = createSongRequest.Artist,
                BPM = createSongRequest.BPM,
                Tone = createSongRequest.Tone
                // No necesitamos asignar CoverImage ni ZipFile aquí, ya que esos se manejarán por separado (subidos como archivos)
            };
        }
    }
}
