namespace api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }  // Cambié de Password a PasswordHash
        public string Email { get; set; }  // Agregado campo Email
    }
}
