using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TP6_Servicios_API_REST.Data;
using TP6_Shared.DTOs.Proveedor.Request;
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

        // 1. Registro PÚBLICO: Clientes / Usuarios estándar
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
                Rol = Roles.Usuario,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Cuenta de Usuario creada correctamente." });
        }

        // 2. Registro de PROVEEDOR: Crea Usuario (Rol Proveedor) + Entidad Proveedor
        [HttpPost("register-proveedor")]
        public async Task<IActionResult> RegisterProveedor(CrearProveedorRequest dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { mensaje = "El correo electrónico ya está registrado." });

            if (await _context.Proveedores.AnyAsync(p => p.Cuit == dto.Cuit))
                return BadRequest(new { mensaje = "El CUIT ingresado ya se encuentra registrado." });

            using var transaccion = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear el usuario del proveedor
                var usuario = new Usuario
                {
                    NombreCompleto = dto.NombreCompleto,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Rol = Roles.Proveedor,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // 2. Crear el registro del proveedor asociado al usuario
                var proveedor = new Proveedor
                {
                    RazonSocial = dto.RazonSocial,
                    Cuit = dto.Cuit,
                    Telefono = dto.Telefono,
                    Email = dto.Email,
                    UsuarioId = usuario.Id
                };

                _context.Proveedores.Add(proveedor);
                await _context.SaveChangesAsync();

                await transaccion.CommitAsync();

                return Ok(new { mensaje = "Cuenta de Proveedor registrada exitosamente." });
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = $"Error al registrar proveedor: {ex.Message}" });
            }
        }

        // 3. Registro PROTEGIDO: Solo un Administrador puede crear otro Administrador
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
            // Incluimos la navegación al Proveedor para conocer su Id si corresponde
            var usuario = await _context.Usuarios
                .Include(u => u.Proveedor)
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return Unauthorized(new { mensaje = "Credenciales inválidas." });

            var token = GenerarJwtToken(usuario);
            return Ok(new { token });
        }

        private string GenerarJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            string secretKey = jwtSettings["Key"]
                ?? throw new InvalidOperationException("La clave secreta JWT no está configurada en los secretos de la aplicación.");

            var key = Encoding.UTF8.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol ?? Roles.Cliente)
            };

            // Si es Proveedor y tiene perfil asignado, sumamos el ProveedorId al token
            if (usuario.Rol == Roles.Proveedor && usuario.Proveedor != null)
            {
                claims.Add(new Claim("ProveedorId", usuario.Proveedor.Id.ToString()));
            }

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