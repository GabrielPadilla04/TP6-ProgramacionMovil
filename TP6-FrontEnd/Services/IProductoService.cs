using System;
using System.Collections.Generic;
using System.Text;
using TP6_Shared.DTOs.Producto.Request;
using TP6_Shared.DTOs.Producto.Response;


namespace TP6_FrontEnd.Services
{
    public interface IProductoService
    {
        //Métodos Lectura (públicos/clientes)
        Task<List<ProductoListadoResponse>> ObtenerTodosAsync(int page, int pageSize);
        Task<ProductoResponse> ObtenerPorIdAsync(int id);

        //Métodos Admin (van a pedir JWT)
        Task<ProductoResponse> CrearAsync(CrearProductoRequest request);
        Task<bool> ActualizarAsync(int id, ActualizarProductoRequest request);
        Task<bool> EliminarAsync(int id);

        // MAUI no usa IFormFile de ASP.NET, así que lo adaptamos para enviar un Stream o byte[]
        Task<ProductoImagenResponse> SubirImagenAsync(int id, Stream imagenStream, string nombreArchivo);
    }
}
