namespace TP6_Shared.DTOs
{
    public class CrearSalidaRequest
    {
        public string? ClienteNombre { get; set; }
        public string? ClienteEmail { get; set; }
        public List<CrearSalidaDetalleItemRequest> Detalles { get; set; } = new();
    }

    public class CrearSalidaDetalleItemRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
    }
}