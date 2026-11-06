namespace Evaluacion_ArticulosJEAC.Api.Exceptions;

public class AccesoDenegadoException : Exception
{
    public AccesoDenegadoException(string mensaje) : base(mensaje)
    {
    }
}
