using Evaluacion_ArticulosJEAC.Api.Dtos;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public interface IGrupoServicio
{
    Task<List<GrupoDto>> ListarAsync();
    Task<GrupoDto> CrearAsync(CrearGrupoDto dto);
    Task EliminarAsync(int id);
}
