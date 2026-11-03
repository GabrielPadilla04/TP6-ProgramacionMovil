using TP6_Shared.DTOs.Usuario.Output;

namespace TP6_FrontEnd.Services
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDtoOutput>> ObtenerTodosAsync();
        Task<bool> EliminarAsync(int id);
    }
}