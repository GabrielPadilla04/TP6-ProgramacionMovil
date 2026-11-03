using TP6_Shared.DTOs;
using TP6_Shared.DTOs.Salida.Response;

namespace TP6_FrontEnd.Services
{
    public interface ISalidaService
    {
        Task<List<SalidaListadoResponse>> ObtenerTodasAsync();
    }
}