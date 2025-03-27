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
    }
}
