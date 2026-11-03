using System.Net.Http.Json;
using TP6_Shared.DTOs.Usuario.Output;

namespace TP6_FrontEnd.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HttpClient _http;

        public UsuarioService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<UsuarioDtoOutput>> ObtenerTodosAsync()
        {
            var respuesta = await _http.GetFromJsonAsync<List<UsuarioDtoOutput>>("api/Usuarios");
            return respuesta ?? new List<UsuarioDtoOutput>();
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var respuesta = await _http.DeleteAsync($"api/Usuarios/{id}");
            return respuesta.IsSuccessStatusCode;
        }
    }
}