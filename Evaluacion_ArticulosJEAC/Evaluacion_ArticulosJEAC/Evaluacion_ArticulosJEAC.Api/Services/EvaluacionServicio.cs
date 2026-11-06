using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Api.Mappers;
using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public class EvaluacionServicio : IEvaluacionServicio
{
    public const int MinimoEvaluadoresPorGrupo = 2;

    private readonly IRepositorio<Evaluacion> _evaluaciones;
    private readonly IRepositorio<GrupoExposicion> _grupos;
    private readonly IRepositorio<Docente> _docentes;

    public EvaluacionServicio(
        IRepositorio<Evaluacion> evaluaciones,
        IRepositorio<GrupoExposicion> grupos,
        IRepositorio<Docente> docentes)
    {
        _evaluaciones = evaluaciones;
        _grupos = grupos;
        _docentes = docentes;
    }

    private IQueryable<Evaluacion> ConsultaCompleta()
    {
        return _evaluaciones.Consultar()
            .Include(e => e.Evaluador)
            .Include(e => e.Grupo).ThenInclude(g => g.Articulo)
            .Include(e => e.Grupo).ThenInclude(g => g.Asesor)
            .Include(e => e.Grupo).ThenInclude(g => g.Estudiantes);
    }

    public async Task<List<EvaluacionDto>> ListarPorDocenteAsync(int docenteId)
    {
        var evaluaciones = await ConsultaCompleta()
            .AsNoTracking()
            .Where(e => e.DocenteEvaluadorId == docenteId)
            .ToListAsync();

        return evaluaciones
            .OrderBy(e => e.Grupo.Orden)
            .Select(e => Mapeador.AEvaluacionDto(e))
            .ToList();
    }

    public async Task<List<EvaluacionDto>> ListarTodasAsync()
    {
        var evaluaciones = await ConsultaCompleta()
            .AsNoTracking()
            .ToListAsync();

        return evaluaciones
            .OrderBy(e => e.Grupo.Orden)
            .ThenBy(e => e.Evaluador.Nombre)
            .Select(e => Mapeador.AEvaluacionDto(e))
            .ToList();
    }

    public async Task<EvaluacionDto> AsignarEvaluadorAsync(AsignarEvaluadorDto dto)
    {
        var grupo = await _grupos.ObtenerPorIdAsync(dto.GrupoId)
            ?? throw new RecursoNoEncontradoException("El grupo no existe.");

        var docente = await _docentes.ObtenerPorIdAsync(dto.DocenteId)
            ?? throw new RecursoNoEncontradoException("El docente no existe.");

        if (grupo.AsesorId == docente.Id)
        {
            throw new ReglaNegocioException("El asesor del grupo no puede ser su evaluador.");
        }

        var yaAsignado = await _evaluaciones.Consultar()
            .AnyAsync(e => e.GrupoExposicionId == grupo.Id && e.DocenteEvaluadorId == docente.Id);

        if (yaAsignado)
        {
            throw new ReglaNegocioException("El docente ya es evaluador de este grupo.");
        }

        var evaluacion = new Evaluacion
        {
            GrupoExposicionId = grupo.Id,
            DocenteEvaluadorId = docente.Id
        };

        await _evaluaciones.AgregarAsync(evaluacion);
        await _evaluaciones.GuardarCambiosAsync();

        var creada = await ConsultaCompleta().AsNoTracking().FirstAsync(e => e.Id == evaluacion.Id);
        return Mapeador.AEvaluacionDto(creada);
    }

    public async Task<EvaluacionDto> CalificarAsync(int evaluacionId, int docenteId, CalificarDto dto)
    {
        var evaluacion = await _evaluaciones.ObtenerPorIdAsync(evaluacionId)
            ?? throw new RecursoNoEncontradoException("La evaluación no existe.");

        if (evaluacion.DocenteEvaluadorId != docenteId)
        {
            throw new AccesoDenegadoException("Solo puede calificar los artículos que le fueron asignados.");
        }

        var nota = dto.Nota ?? throw new ReglaNegocioException("La nota es obligatoria.");

        evaluacion.Calificar(nota, dto.Observaciones);
        await _evaluaciones.GuardarCambiosAsync();

        var actualizada = await ConsultaCompleta().AsNoTracking().FirstAsync(e => e.Id == evaluacionId);
        return Mapeador.AEvaluacionDto(actualizada);
    }

    public async Task EliminarAsignacionAsync(int evaluacionId)
    {
        var evaluacion = await _evaluaciones.ObtenerPorIdAsync(evaluacionId)
            ?? throw new RecursoNoEncontradoException("La evaluación no existe.");

        if (evaluacion.EstaCalificada)
        {
            throw new ReglaNegocioException("No se puede eliminar una evaluación que ya fue calificada.");
        }

        _evaluaciones.Eliminar(evaluacion);
        await _evaluaciones.GuardarCambiosAsync();
    }

    public async Task<List<ResultadoGrupoDto>> ObtenerResultadosAsync()
    {
        var grupos = await _grupos.Consultar()
            .AsNoTracking()
            .Include(g => g.Articulo)
            .Include(g => g.Evaluaciones)
            .OrderBy(g => g.Orden)
            .ToListAsync();

        return grupos.Select(CrearResultado).ToList();
    }

    private static ResultadoGrupoDto CrearResultado(GrupoExposicion grupo)
    {
        var total = grupo.Evaluaciones.Count;
        var calificadas = grupo.Evaluaciones.Where(e => e.Nota.HasValue).ToList();

        decimal? promedio = calificadas.Count > 0
            ? decimal.Round(calificadas.Average(e => e.Nota!.Value), 2)
            : null;

        string estado;
        if (total < MinimoEvaluadoresPorGrupo)
        {
            estado = "Faltan evaluadores";
        }
        else if (calificadas.Count == total)
        {
            estado = "Consolidado";
        }
        else
        {
            estado = "En evaluación";
        }

        return new ResultadoGrupoDto
        {
            GrupoId = grupo.Id,
            GrupoNombre = grupo.Nombre,
            ArticuloTitulo = grupo.Articulo.Titulo,
            TotalEvaluadores = total,
            Calificadas = calificadas.Count,
            Promedio = promedio,
            Estado = estado
        };
    }
}
