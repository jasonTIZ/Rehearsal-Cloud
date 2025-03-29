namespace api.Dtos.Song
{
    public class CreateSongRequestDto
    {
        public string SongName { get; set; }
        public string Artist { get; set; }
        public int BPM { get; set; }
        public string Tone { get; set; }
        public IFormFile CoverImage { get; set; } // Representación de la imagen de portada (subida por el usuario)
        public IFormFile ZipFile { get; set; } // El archivo .zip que contendrá los archivos de audio
    }
}
