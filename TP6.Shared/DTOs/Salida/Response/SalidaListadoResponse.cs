
namespace TP6_Shared.DTOs.Salida.Response
{
  public class SalidaListadoResponse
    {
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public string? ClienteNombre { get; set; }
    public string? ClienteEmail { get; set; }
    public int CantidadItems { get; set; }
    public List<SalidaDetalleItemResponse> Detalles { get; set; } = new();
    }

    public class SalidaDetalleItemResponse
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string? ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Subtotal { get; set; }
    }
}
