using System.Net.Http.Headers;
using System.Net.Http.Json;
using TP6_Shared.DTOs.Producto.Request;
using TP6_Shared.DTOs.Producto.Response;

namespace TP6_FrontEnd.Services
{
    public class ProductoService : IProductoService
    {
        private readonly HttpClient _httpClient;

        public ProductoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ----------------------------------------------------
        // LECTURA (PÚBLICOS / CLIENTES)
        // ----------------------------------------------------

        public async Task<List<ProductoListadoResponse>> ObtenerTodosAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ProductoListadoResponse>>(
                    $"api/productos?page={page}&pageSize={pageSize}");

                return response ?? new List<ProductoListadoResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<ProductoListadoResponse>();
            }
        }

        public async Task<ProductoResponse?> ObtenerPorIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductoResponse>($"api/productos/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener producto {id}: {ex.Message}");
                return null;
            }
        }

        // ----------------------------------------------------
        // GESTIÓN / ADMIN (REQUIEREN TOKEN JWT)
        // ----------------------------------------------------

        public async Task<ProductoResponse?> CrearAsync(CrearProductoRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/productos", request);
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<ProductoResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear producto: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ActualizarAsync(int id, ActualizarProductoRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/productos/{id}", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar producto {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/productos/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar producto {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<ProductoImagenResponse?> SubirImagenAsync(int id, Stream imagenStream, string nombreArchivo)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(imagenStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                // "imagen" es el nombre habitual del parámetro en el controlador de la API
                content.Add(streamContent, "imagen", nombreArchivo);

                var response = await _httpClient.PostAsync($"api/productos/{id}/imagen", content);
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<ProductoImagenResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir imagen del producto {id}: {ex.Message}");
                return null;
            }
        }
    }
}