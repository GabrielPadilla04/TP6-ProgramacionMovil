using System.Net.Http.Json;
using TP6_Shared.DTOs;
using TP6_Shared.DTOs.Salida.Response;

namespace TP6_FrontEnd.Services
{
    public class SalidaService : ISalidaService
    {
        private readonly HttpClient _http;

        public SalidaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<SalidaListadoResponse>> ObtenerTodasAsync()
        {
            var respuesta = await _http.GetFromJsonAsync<List<SalidaListadoResponse>>("api/Salidas");
            return respuesta ?? new List<SalidaListadoResponse>();
        }
    }
}