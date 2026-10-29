using System.Net.Http.Json;
using TP6_Shared.DTOs;

namespace TP6_FrontEnd.Services
{
    public class VentaService : IVentaService
    {
        private readonly HttpClient _httpClient;

        public VentaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegistrarSalidaAsync(RegistrarSalidaDto request)
        {
            try
            {
                // Apunta al endpoint de SalidasController de tu API
                var response = await _httpClient.PostAsJsonAsync("api/salidas", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar la salida/venta: {ex.Message}");
                return false;
            }
        }
    }
}