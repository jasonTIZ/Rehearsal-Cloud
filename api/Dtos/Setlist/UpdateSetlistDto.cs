namespace api.Dtos.Setlist
{
    public class UpdateSetlistRequestDto
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }

        // Nullable para indicar que no se requiere modificar canciones si es null
        public List<int>? SetlistSongs { get; set; }
    }
}