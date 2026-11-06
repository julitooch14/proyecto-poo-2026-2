using Evaluacion_ArticulosJEAC.Api.Dtos;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public interface IEvaluacionServicio
{
    Task<List<EvaluacionDto>> ListarPorDocenteAsync(int docenteId);
    Task<List<EvaluacionDto>> ListarTodasAsync();
    Task<EvaluacionDto> AsignarEvaluadorAsync(AsignarEvaluadorDto dto);
    Task<EvaluacionDto> CalificarAsync(int evaluacionId, int docenteId, CalificarDto dto);
    Task EliminarAsignacionAsync(int evaluacionId);
    Task<List<ResultadoGrupoDto>> ObtenerResultadosAsync();
}
