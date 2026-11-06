using Evaluacion_ArticulosJEAC.Api.Dtos;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public interface IDocenteServicio
{
    Task<List<DocenteDto>> ListarAsync();
    Task<DocenteDto> CrearAsync(CrearDocenteDto dto);
    Task EliminarAsync(int id);
}
