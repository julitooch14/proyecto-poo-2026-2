using Evaluacion_ArticulosJEAC.Api.Dtos;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public interface IAutenticacionServicio
{
    Task<RespuestaLoginDto> AutenticarAsync(SolicitudLoginDto solicitud);
}
