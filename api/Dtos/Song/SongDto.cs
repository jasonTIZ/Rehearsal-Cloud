namespace api.Dtos.Song
{
    public class SongDto
    {
        public int Id { get; set; }
        public string SongName { get; set; }
        public string Artist { get; set; }
        public int BPM { get; set; }
        public string Tone { get; set; }
        public string CoverImage { get; set; } // Ruta de la imagen de portada
        public DateTime CreatedAt { get; set; }
    }
}