namespace TP6_Servicios_API_REST.Excepciones
{
    public class RecursoExistenteException : Exception
    {
        public RecursoExistenteException(string mensaje) : base(mensaje)
        {
        }
    }
}
