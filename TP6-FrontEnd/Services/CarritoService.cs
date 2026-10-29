using TP6_Shared.DTOs.Producto.Response;
using TP6_FrontEnd.Models;

namespace TP6_FrontEnd.Services
{
    public class CarritoService : ICarritoService
    {
        private List<ItemCarrito> _items = new();

        public event Action? OnCarritoCambiado;

        public List<ItemCarrito> ObtenerItems() => _items;

        public void AgregarItem(ProductoResponse producto, int cantidad)
        {
            var itemExistente = _items.FirstOrDefault(i => i.ProductoId == producto.Id);

            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                _items.Add(new ItemCarrito
                {
                    ProductoId = producto.Id,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });
            }

            NotificarCambios();
        }

        public void ActualizarCantidad(int productoId, int cantidad)
        {
            var item = _items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                if (cantidad <= 0)
                {
                    _items.Remove(item);
                }
                else
                {
                    item.Cantidad = cantidad;
                }
            }
        }
        public void EliminarItem(int productoId)
        {
            var item = _items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                _items.Remove(item);
                NotificarCambios();
            }
        }

        public void VaciarCarrito()
        {
            _items.Clear();
            NotificarCambios();
        }

        public decimal ObtenerTotal() => _items.Sum(i => i.Subtotal);

        private void NotificarCambios() => OnCarritoCambiado?.Invoke();
    }
}