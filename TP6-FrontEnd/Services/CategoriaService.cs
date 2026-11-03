using System.Net.Http.Json;
using TP6_Shared.DTOs.Categoria.Response;

namespace TP6_FrontEnd.Services;

public interface ICategoriaService
{
    Task<List<CategoriaResponse>> ObtenerTodasAsync();
}

public class CategoriaService : ICategoriaService
{
    private readonly HttpClient _http;

    public CategoriaService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<CategoriaResponse>> ObtenerTodasAsync()
    {
        try
        {
            var respuesta = await _http.GetFromJsonAsync<List<CategoriaResponse>>("api/Categorias");
            return respuesta ?? new List<CategoriaResponse>();
        }
        catch
        {
            return new List<CategoriaResponse>();
        }
    }
}