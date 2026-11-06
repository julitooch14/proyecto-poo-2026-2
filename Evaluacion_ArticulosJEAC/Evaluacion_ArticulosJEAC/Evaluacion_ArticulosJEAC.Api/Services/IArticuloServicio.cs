using Evaluacion_ArticulosJEAC.Api.Dtos;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public interface IArticuloServicio
{
    Task<List<ArticuloDto>> ListarAsync();
    Task<ArticuloDto> CrearAsync(CrearArticuloDto dto);
    Task EliminarAsync(int id);
}
