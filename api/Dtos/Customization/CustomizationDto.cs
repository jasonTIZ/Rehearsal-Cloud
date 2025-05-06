namespace api.Dtos.Customization
{
    public class CustomizationDto
    {
        public int Id { get; set; }
        public int SongId { get; set; }
        public string VolumeLevel { get; set; }
        public string EqualizationSettings { get; set; }
        public bool LoopEnabled { get; set; }
        public bool FadeIn { get; set; }
        public bool FadeOut { get; set; }
    }
}