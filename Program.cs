using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TP6_Servicios_API_REST.Data;
using TP6_Servicios_API_REST.Middlewares;
using TP6_Servicios_API_REST.Models;
using TP6_Servicios_API_REST.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Base de datos (MySQL con versión fija)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30))));

// 2. Autenticación y Autorización (JWT)
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("La clave secreta JWT no está configurada en los secretos de la aplicación.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Permisivo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 4. Inyección de Dependencias (Servicios)
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ImagenService>();

// 5. Controladores y OpenAPI NATIVO de .NET
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// 6. Semilla del Administrador Inicial (usando secretos de usuario)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    if (!context.Usuarios.Any(u => u.Rol == Roles.Administrador))
    {
        var adminNombre = config["AdminInicial:Nombre"] ?? "Administrador General";
        var adminEmail = config["AdminInicial:Email"] ?? "admin@tienda.com";
        var adminPassword = config["AdminInicial:Password"]
            ?? throw new InvalidOperationException("No se configuró la contraseña inicial del administrador en secrets.json.");

        context.Usuarios.Add(new Usuario
        {
            NombreCompleto = adminNombre,
            Email = adminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Rol = Roles.Administrador,
            FechaCreacion = DateTime.UtcNow
        });

        context.SaveChanges();
    }
}

// 7. Pipeline HTTP
app.UseMiddleware<AuditoriaMiddleware>();

app.MapOpenApi();
app.MapScalarApiReference();

// Redirecciona automáticamente la raíz (/) directamente hacia la interfaz de Scalar
app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("Permisivo");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();