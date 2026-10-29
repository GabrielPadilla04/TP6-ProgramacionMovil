using TP6_Shared.DTOs.Producto.Response;
using TP6_FrontEnd.Models;

namespace TP6_FrontEnd.Services
{
    public interface ICarritoService
    {
        List<ItemCarrito> ObtenerItems();
        void AgregarItem(ProductoResponse producto, int cantidad);
        void EliminarItem(int productoId);
        void VaciarCarrito();
        decimal ObtenerTotal();
        void ActualizarCantidad(int productoId, int cantidad);
        event Action OnCarritoCambiado;
    }
}