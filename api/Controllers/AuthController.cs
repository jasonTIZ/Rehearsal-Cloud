using api.Data;
using api.Dtos.User;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly byte[] _key = Encoding.UTF8.GetBytes("your-secret-key"); // Clave secreta fija

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Auth/register
      [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] CreateUserRequestDto createUserRequest)
{
    if (createUserRequest.Password.Length < 8)
    {
        return BadRequest("La contraseña debe tener al menos 8 caracteres.");
    }

    if (await _context.Users.AnyAsync(u => u.Username == createUserRequest.Username || u.Email == createUserRequest.Email))
    {
        return BadRequest("El nombre de usuario o correo electrónico ya está en uso.");
    }

    using var hmac = new HMACSHA256(_key);
    var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(createUserRequest.Password)));

    var user = createUserRequest.ToUserFromCreateDto(passwordHash);

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok("Usuario registrado exitosamente.");
}

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);
            if (user == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            using var hmac = new HMACSHA256(_key);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(loginRequest.Password)));

            if (computedHash != user.PasswordHash)
            {
                return Unauthorized("Contraseña incorrecta.");
            }

            return Ok("Inicio de sesión exitoso.");
        }

        // GET: api/Auth/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => u.ToDto())
                .ToListAsync();

            return Ok(users);
        }

       // DELETE: api/Auth/users/{id}
[HttpDelete("users/{id}")]
public async Task<IActionResult> DeleteUser(int id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
        return NotFound(new DeleteUserRequestDto
        {
            Id = id,
            Message = "Usuario no encontrado."
        });
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();

    return Ok(new DeleteUserRequestDto
    {
        Id = id,
        Message = $"Usuario con ID {id} eliminado exitosamente."
    });
}
    }
}