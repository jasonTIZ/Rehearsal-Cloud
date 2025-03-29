using Microsoft.EntityFrameworkCore;
using api.Models;  // Para el modelo User

namespace api.Data  // El espacio de nombres debe ser 'api.Data'
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }  // Asegúrate de tener este DbSet

        public DbSet<Email> Emails { get; set; } // Asegúrate de que este DbSet coincida con tu tabla
    
         public DbSet<Playlist> Playlists { get; set; } 
    }
}
