namespace TP6_Shared.DTOs.Usuario.Output
{
    public class UsuarioAuthDtoOutput
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

        // Constructor vacío necesario para deserialización JSON
        public UsuarioAuthDtoOutput() { }

        // Constructor de 4 argumentos 
        public UsuarioAuthDtoOutput(int id, string nombreCompleto, string email, string rol)
        {
            Id = id;
            NombreCompleto = nombreCompleto;
            Email = email;
            Rol = rol;
        }

        // Constructor completo con Token
        public UsuarioAuthDtoOutput(int id, string nombreCompleto, string email, string rol, string token)
            : this(id, nombreCompleto, email, rol)
        {
            Token = token;
        }
    }
}