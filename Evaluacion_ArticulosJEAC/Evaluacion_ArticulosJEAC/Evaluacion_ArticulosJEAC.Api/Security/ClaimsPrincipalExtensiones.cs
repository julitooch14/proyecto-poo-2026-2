using System.Security.Claims;
using Evaluacion_ArticulosJEAC.Api.Exceptions;

namespace Evaluacion_ArticulosJEAC.Api.Security;

public static class ClaimsPrincipalExtensiones
{
    public static int ObtenerDocenteId(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirst("docenteId")?.Value;
        if (!int.TryParse(valor, out var docenteId))
        {
            throw new AccesoDenegadoException("El usuario autenticado no está asociado a un docente.");
        }

        return docenteId;
    }
}
