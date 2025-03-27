using api.Data;  // Asegúrate de importar ApplicationDbContext desde el espacio de nombres adecuado
using api.Models;  // Para la clase User
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;  // Cambié AppDbContext por ApplicationDbContext

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Métodos del controlador
    }
}
