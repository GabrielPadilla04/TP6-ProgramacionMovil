using TP6_Shared.DTOs.Producto.Request;
using TP6_Shared.DTOs.Producto.Response;

namespace TP6_FrontEnd.Services
{
    public interface IProductoService
    {
        // Acá está el cambio: agregamos el bool soloMios = false
        Task<List<ProductoListadoResponse>> ObtenerTodosAsync(int page = 1, int pageSize = 10, bool soloMios = false);

        Task<ProductoResponse?> ObtenerPorIdAsync(int id);

        Task<ProductoResponse?> CrearAsync(CrearProductoRequest request);

        Task<bool> ActualizarAsync(int id, ActualizarProductoRequest request);

        Task<bool> EliminarAsync(int id);

        Task<ProductoImagenResponse?> SubirImagenAsync(int id, Stream imagenStream, string nombreArchivo);
    }
}