using Evaluacion_ArticulosJEAC.Api.Dtos;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public interface IEstudianteServicio
{
    Task<List<EstudianteDto>> ListarAsync();
    Task<EstudianteDto> CrearAsync(CrearEstudianteDto dto);
    Task EliminarAsync(int id);
}
