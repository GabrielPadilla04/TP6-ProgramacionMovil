using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP6_Servicios_API_REST.Data;
using TP6_Shared.DTOs;
using TP6_Servicios_API_REST.Models;

namespace TP6_Servicios_API_REST.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SalidasController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalidasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Cliente},{Roles.Usuario}")]// Restringido únicamente a Administradores
    public async Task<IActionResult> RegistrarSalida([FromBody] RegistrarSalidaDto dto)
    {
        if (dto.Detalles == null || !dto.Detalles.Any())
            return BadRequest("Debe incluir al menos un producto en la venta.");

        // 1. Obtener y validar el ID del usuario desde los Claims del Token
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("sub")?.Value
                             ?? User.FindFirst("id")?.Value;

        if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out int usuarioId))
        {
            return Unauthorized("Token no válido o identificador de usuario ausente.");
        }

        // 2. Iniciar la transacción para operaciones atómicas
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var salida = new Salida
            {
                ClienteId = usuarioId,
                Fecha = DateTime.UtcNow,
                Total = 0,
                Detalles = new List<SalidaDetalle>()
            };

            decimal totalSalida = 0;

            foreach (var item in dto.Detalles)
            {
                var producto = await _context.Productos.FindAsync(item.ProductoId);
                if (producto == null)
                {
                    await transaction.RollbackAsync();
                    return NotFound($"El producto con ID {item.ProductoId} no existe.");
                }

                // Control estricto de stock
                if (producto.Stock < item.Cantidad)
                {
                    await transaction.RollbackAsync();
                    return BadRequest($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {item.Cantidad}.");
                }

                // Descuento de stock
                producto.Stock -= item.Cantidad;

                // Se toma el precio oficial del producto directamente desde la base de datos
                var subtotal = item.Cantidad * producto.Precio;
                totalSalida += subtotal;

                salida.Detalles.Add(new SalidaDetalle
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioVenta = producto.Precio
                });
            }

            salida.Total = totalSalida;

            _context.Salidas.Add(salida);

            // Guarda tanto la salida como la actualización del stock en Productos
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                mensaje = "Venta registrada y stock descontado con éxito.",
                salidaId = salida.Id,
                total = salida.Total
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Error interno al registrar la venta: {ex.Message}");
        }
    }
}