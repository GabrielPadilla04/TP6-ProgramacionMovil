using System.ComponentModel.DataAnnotations.Schema;

namespace TP6_Servicios_API_REST.Models
{
    public class Salida
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Usuario Cliente { get; set; } = null!;


        // Detalle de venta
        public ICollection<SalidaDetalle> Detalles { get; set; } = new List<SalidaDetalle>();
    }
}