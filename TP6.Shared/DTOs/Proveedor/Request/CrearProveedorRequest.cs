using System.ComponentModel.DataAnnotations;

namespace TP6_Shared.DTOs.Proveedor.Request
{
    public class CrearProveedorRequest
    {
        [Required(ErrorMessage = "El nombre de contacto es obligatorio")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio"), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria"), MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La razón social es obligatoria")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CUIT es obligatorio")]
        public string Cuit { get; set; } = string.Empty;

        public string? Telefono { get; set; }
    }
}