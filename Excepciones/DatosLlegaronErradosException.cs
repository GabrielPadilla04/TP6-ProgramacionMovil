namespace TP6_Servicios_API_REST.Excepciones
{
    public class DatosLlegaronErradosException : Exception
    {
        public DatosLlegaronErradosException(string mensaje) : base(mensaje)
        {
        }
    }
}
