using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP6_Servicios_API_REST.Models
{
    [Table("Carritos")]
    public class Carrito
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        public ICollection<ItemCarrito> Items { get; set; } = new List<ItemCarrito>();
    }
}