namespace TP6_Shared.DTOs.Producto.Response
{
    public record ProductoImagenResponse(
        int Id,
        string NombreOriginal,
        string Url,
        string TipoContenido,
        long TamanoBytes,
        DateTime FechaCreacion
    );
}