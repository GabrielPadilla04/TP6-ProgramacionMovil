using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("LogsAuditoria")]
    public class LogAuditoria
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)]
        public string Nivel { get; set; } = "Info"; // Info, Warning, Error

        [Required]
        public string Mensaje { get; set; } = string.Empty;

        public string? StackTrace { get; set; }

        [MaxLength(250)]
        public string? Endpoint { get; set; }

        public int? IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }
    }
}