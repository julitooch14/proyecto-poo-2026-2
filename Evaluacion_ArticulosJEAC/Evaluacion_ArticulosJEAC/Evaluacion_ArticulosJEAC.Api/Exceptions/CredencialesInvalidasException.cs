namespace Evaluacion_ArticulosJEAC.Api.Exceptions;

public class CredencialesInvalidasException : Exception
{
    public CredencialesInvalidasException() : base("Usuario o contraseña incorrectos.")
    {
    }
}
