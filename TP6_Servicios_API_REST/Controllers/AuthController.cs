using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TP6_Servicios_API_REST.Data;
using TP6_Shared.DTOs.Auth;
using TP6_Servicios_API_REST.Models;

namespace TP6_Servicios_API_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 1. Registro PÚBLICO: Todo aquel que se registre por acá es estrictamente CLIENTE
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });

            var usuario = new Usuario
            {
                NombreCompleto = dto.NombreCompleto,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = Roles.Usuario, // Asignación fija y segura para el flujo normal
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cuenta de Usuario creada correctamente." });
        }

        // 2. Registro PROTEGIDO: Solo un Administrador autenticado puede crear otro Administrador
        [HttpPost("register-admin")]
        [Authorize(Roles = Roles.Administrador)]
        public async Task<IActionResult> RegisterAdmin(RegisterDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });

            var nuevoAdmin = new Usuario
            {
                NombreCompleto = dto.NombreCompleto,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Rol = Roles.Administrador,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(nuevoAdmin);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Administrador registrado correctamente." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return Unauthorized(new { mensaje = "Credenciales inválidas." });

            var token = GenerarJwtToken(usuario);
            return Ok(new { token });
        }

        private string GenerarJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            // Lee la clave del entorno/secrets. Si no existe, lanza excepción en lugar de usar un string hardcodeado
            string secretKey = jwtSettings["Key"]
                ?? throw new InvalidOperationException("La clave secreta JWT no está configurada en los secretos de la aplicación.");

            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol ?? Roles.Cliente)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = jwtSettings["Issuer"] ?? "TP6Api",
                Audience = jwtSettings["Audience"] ?? "TP6Clients",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}