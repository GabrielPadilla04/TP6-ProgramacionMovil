using TP6_Shared.DTOs;

namespace TP6_FrontEnd.Services
{
    public interface IVentaService
    {
        Task<bool> RegistrarSalidaAsync(RegistrarSalidaDto request);
    }
}