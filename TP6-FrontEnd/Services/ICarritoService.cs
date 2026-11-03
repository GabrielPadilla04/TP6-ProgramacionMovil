using TP6_FrontEnd.Services;

public interface ICarritoService
{
    event Action? OnChange;
    List<CartItem> Items { get; }
    void AgregarProducto(int id, string nombre, decimal precio, int stock, string? imagenUrl, int cantidad = 1);
    void IncrementarCantidad(int productoId);
    void DecrementarCantidad(int productoId);
    void RemoverProducto(int productoId);
    void VaciarCarrito();
    decimal ObtenerTotal();
    int ObtenerCantidadTotalItems();
}