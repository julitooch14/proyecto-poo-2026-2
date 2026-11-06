using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Api.Mappers;
using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public class GrupoServicio : IGrupoServicio
{
    private readonly IRepositorio<GrupoExposicion> _grupos;
    private readonly IRepositorio<ArticuloCientifico> _articulos;
    private readonly IRepositorio<Docente> _docentes;
    private readonly IRepositorio<Estudiante> _estudiantes;

    public GrupoServicio(
        IRepositorio<GrupoExposicion> grupos,
        IRepositorio<ArticuloCientifico> articulos,
        IRepositorio<Docente> docentes,
        IRepositorio<Estudiante> estudiantes)
    {
        _grupos = grupos;
        _articulos = articulos;
        _docentes = docentes;
        _estudiantes = estudiantes;
    }

    private IQueryable<GrupoExposicion> ConsultaCompleta()
    {
        return _grupos.Consultar()
            .Include(g => g.Articulo)
            .Include(g => g.Asesor)
            .Include(g => g.Estudiantes)
            .Include(g => g.Evaluaciones);
    }

    public async Task<List<GrupoDto>> ListarAsync()
    {
        var grupos = await ConsultaCompleta()
            .AsNoTracking()
            .OrderBy(g => g.Orden)
            .ToListAsync();

        return grupos.Select(g => Mapeador.AGrupoDto(g)).ToList();
    }

    public async Task<GrupoDto> CrearAsync(CrearGrupoDto dto)
    {
        var articulo = await _articulos.ObtenerPorIdAsync(dto.ArticuloId)
            ?? throw new RecursoNoEncontradoException("El artículo seleccionado no existe.");

        var asesor = await _docentes.ObtenerPorIdAsync(dto.AsesorId)
            ?? throw new RecursoNoEncontradoException("El docente asesor seleccionado no existe.");

        if (await _grupos.Consultar().AnyAsync(g => g.ArticuloCientificoId == articulo.Id))
        {
            throw new ReglaNegocioException("El artículo ya está asignado a otro grupo.");
        }

        var ids = dto.EstudianteIds.Distinct().ToList();
        var estudiantes = await _estudiantes.Consultar().Where(e => ids.Contains(e.Id)).ToListAsync();

        if (estudiantes.Count != ids.Count)
        {
            throw new RecursoNoEncontradoException("Alguno de los estudiantes seleccionados no existe.");
        }

        if (estudiantes.Any(e => e.GrupoExposicionId.HasValue))
        {
            throw new ReglaNegocioException("Alguno de los estudiantes ya pertenece a un grupo.");
        }

        var grupo = new GrupoExposicion
        {
            Nombre = dto.Nombre.Trim(),
            ArticuloCientificoId = articulo.Id,
            AsesorId = asesor.Id,
            Orden = dto.Orden,
            FechaExposicion = dto.FechaExposicion,
            Estudiantes = estudiantes
        };

        await _grupos.AgregarAsync(grupo);
        await _grupos.GuardarCambiosAsync();

        var creado = await ConsultaCompleta().AsNoTracking().FirstAsync(g => g.Id == grupo.Id);
        return Mapeador.AGrupoDto(creado);
    }

    public async Task EliminarAsync(int id)
    {
        var grupo = await ConsultaCompleta().FirstOrDefaultAsync(g => g.Id == id)
            ?? throw new RecursoNoEncontradoException("El grupo no existe.");

        if (grupo.Evaluaciones.Count > 0)
        {
            throw new ReglaNegocioException("No se puede eliminar un grupo que ya tiene evaluadores asignados.");
        }

        _grupos.Eliminar(grupo);
        await _grupos.GuardarCambiosAsync();
    }
}
