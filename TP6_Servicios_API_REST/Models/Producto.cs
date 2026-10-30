using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP6_Servicios_API_REST.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public int Stock { get; set; } = 0;

        [Column(TypeName = "LONGTEXT")]
        public string? ImagenUrl { get; set; }

        // Clave Foránea
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; } = null!;

        // Relación con Imagen
        public int? ImagenId { get; set; }
        public Imagen? Imagen { get; set; }

        // Relaciones con detalles
        public ICollection<IngresoDetalle>? IngresoDetalles { get; set; } = new List<IngresoDetalle>();
        public ICollection<SalidaDetalle>? SalidaDetalles { get; set; } = new List<SalidaDetalle>();
    }
}