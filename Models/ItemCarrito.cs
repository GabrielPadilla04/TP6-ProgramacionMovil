using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("ItemsCarrito")]
    public class ItemCarrito
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdCarrito { get; set; }

        [ForeignKey("IdCarrito")]
        public Carrito? Carrito { get; set; }

        [Required]
        public int IdProducto { get; set; }

        [ForeignKey("IdProducto")]
        public Producto? Producto { get; set; }

        [Required]
        public int Cantidad { get; set; } = 1;
    }
}