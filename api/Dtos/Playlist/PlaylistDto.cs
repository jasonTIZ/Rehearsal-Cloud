namespace api.Dtos.Playlist
{
    public class PlaylistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Songs { get; set; }
    }

    public class CreatePlaylistRequestDto
    {
        public string Name { get; set; }
        public List<string> Songs { get; set; }
    }
}