namespace TP6_FrontEnd.Services
{
    public class CartItem
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public int StockMaximo { get; set; }
        public string? ImagenUrl { get; set; }
        public decimal Subtotal => Precio * Cantidad;
    }

    
    public class CarritoService : ICarritoService
    {
        public event Action? OnChange;
        public List<CartItem> Items { get; private set; } = new();

        public void AgregarProducto(int id, string nombre, decimal precio, int stock, string? imagenUrl, int cantidad = 1)
        {
            var existente = Items.FirstOrDefault(i => i.ProductoId == id);
            if (existente != null)
            {
                if (existente.Cantidad + cantidad <= stock)
                {
                    existente.Cantidad += cantidad;
                }
            }
            else
            {
                Items.Add(new CartItem
                {
                    ProductoId = id,
                    Nombre = nombre,
                    Precio = precio,
                    StockMaximo = stock,
                    ImagenUrl = imagenUrl,
                    Cantidad = Math.Min(cantidad, stock)
                });
            }
            OnChange?.Invoke();
        }

        public void IncrementarCantidad(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null && item.Cantidad < item.StockMaximo)
            {
                item.Cantidad++;
                OnChange?.Invoke();
            }
        }

        public void DecrementarCantidad(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                item.Cantidad--;
                if (item.Cantidad <= 0)
                {
                    Items.Remove(item);
                }
                OnChange?.Invoke();
            }
        }

        public void RemoverProducto(int productoId)
        {
            var item = Items.FirstOrDefault(i => i.ProductoId == productoId);
            if (item != null)
            {
                Items.Remove(item);
                OnChange?.Invoke();
            }
        }

        public void VaciarCarrito()
        {
            Items.Clear();
            OnChange?.Invoke();
        }

        public decimal ObtenerTotal() => Items.Sum(i => i.Subtotal);

        public int ObtenerCantidadTotalItems() => Items.Sum(i => i.Cantidad);
    }
}