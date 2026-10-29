using TP6_Shared.DTOs.Auth;

namespace TP6_FrontEnd.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginDto dto);
        Task LogoutAsync();
        Task<string?> ObtenerTokenAsync();
        Task<int?> ObtenerUsuarioIdAsync();
        Task<string?> ObtenerRolAsync();
        Task<string?> ObtenerEmailAsync();
        Task<bool> EstaAutenticadoAsync();
    }
}