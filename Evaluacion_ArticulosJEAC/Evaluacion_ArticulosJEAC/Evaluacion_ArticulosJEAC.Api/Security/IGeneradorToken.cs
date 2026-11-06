using Evaluacion_ArticulosJEAC.Api.Models;

namespace Evaluacion_ArticulosJEAC.Api.Security;

public interface IGeneradorToken
{
    TokenGenerado Generar(Usuario usuario);
}
