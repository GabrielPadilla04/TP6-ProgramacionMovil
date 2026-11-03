using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TP6_Shared.DTOs.Producto.Request;
using TP6_Shared.DTOs.Producto.Response;
using TP6_Servicios_API_REST.Excepciones;
using TP6_Servicios_API_REST.Models;
using TP6_Servicios_API_REST.Services;

namespace TP6_Servicios_API_REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoService _productoService;
        private readonly ImagenService _imagenService;

        public ProductosController(ProductoService productoService, ImagenService imagenService)
        {
            _productoService = productoService;
            _imagenService = imagenService;
        }

        // Métodos de ayuda para extraer datos del token
        private int ObtenerUsuarioIdActual()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }

        private string ObtenerRolActual()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }

        [HttpGet]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Cliente},{Roles.Usuario},{Roles.Proveedor}")]
        // ACÁ SE AGREGA EL PARÁMETRO soloMios
        public async Task<ActionResult<List<ProductoListadoResponse>>> ObtenerTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool soloMios = false)
        {
            try
            {
                int usuarioId = ObtenerUsuarioIdActual();
                string rol = ObtenerRolActual();

                // SE LE PASA AL SERVICIO
                return Ok(await _productoService.ObtenerTodos(page, pageSize, usuarioId, rol, soloMios));
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Cliente},{Roles.Usuario},{Roles.Proveedor}")]
        public async Task<ActionResult<ProductoResponse>> ObtenerPorId(int id)
        {
            try
            {
                ProductoResponse dto = await _productoService.ObtenerPorId(id);
                return Ok(dto);
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Proveedor}")]
        public async Task<ActionResult<ProductoListadoResponse>> Crear(CrearProductoRequest dto)
        {
            try
            {
                int usuarioId = ObtenerUsuarioIdActual();
                string rol = ObtenerRolActual();

                // Asocia el producto al proveedor correspondiente a este usuario
                ProductoListadoResponse producto = await _productoService.Crear(dto, usuarioId, rol);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("{id}/imagen")]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Proveedor}")]
        public async Task<ActionResult<ProductoImagenResponse>> SubirImagen(int id, IFormFile archivo)
        {
            try
            {
                int usuarioId = ObtenerUsuarioIdActual();
                string rol = ObtenerRolActual();

                // Valida que el proveedor sea el dueño antes de subir la imagen
                await _productoService.ValidarPermisoModificacion(id, usuarioId, rol);

                Imagen imagen = await _imagenService.SubirImagen(id, archivo);

                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                ProductoImagenResponse dto = new ProductoImagenResponse(
                    imagen.Id,
                    imagen.NombreOriginal,
                    $"{baseUrl}/{imagen.RutaRelativa}",
                    imagen.TipoContenido,
                    imagen.TamanoBytes,
                    imagen.FechaCreacion);

                return CreatedAtAction(nameof(ObtenerPorId), new { id }, dto);
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Forbid();
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Proveedor}")]
        public async Task<IActionResult> Actualizar(int id, ActualizarProductoRequest dto)
        {
            try
            {
                int usuarioId = ObtenerUsuarioIdActual();
                string rol = ObtenerRolActual();

                await _productoService.Actualizar(id, dto, usuarioId, rol);
                return NoContent();
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(e.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = $"{Roles.Administrador},{Roles.Proveedor}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                int usuarioId = ObtenerUsuarioIdActual();
                string rol = ObtenerRolActual();

                await _productoService.Eliminar(id, usuarioId, rol);
                return NoContent();
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(e.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}