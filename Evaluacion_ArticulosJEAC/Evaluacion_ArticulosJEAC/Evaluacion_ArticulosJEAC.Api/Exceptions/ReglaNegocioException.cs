namespace Evaluacion_ArticulosJEAC.Api.Exceptions;

public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}
