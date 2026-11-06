using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Api.Mappers;
using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public class EstudianteServicio : IEstudianteServicio
{
    private readonly IRepositorio<Estudiante> _estudiantes;

    public EstudianteServicio(IRepositorio<Estudiante> estudiantes)
    {
        _estudiantes = estudiantes;
    }

    public async Task<List<EstudianteDto>> ListarAsync()
    {
        var estudiantes = await _estudiantes.Consultar()
            .AsNoTracking()
            .Include(e => e.Grupo)
            .OrderBy(e => e.Nombre)
            .ToListAsync();

        return estudiantes.Select(e => Mapeador.AEstudianteDto(e)).ToList();
    }

    public async Task<EstudianteDto> CrearAsync(CrearEstudianteDto dto)
    {
        var codigo = dto.Codigo.Trim().ToUpperInvariant();

        if (await _estudiantes.Consultar().AnyAsync(e => e.Codigo == codigo))
        {
            throw new ReglaNegocioException("Ya existe un estudiante con ese código.");
        }

        var estudiante = new Estudiante
        {
            Nombre = dto.Nombre.Trim(),
            Codigo = codigo
        };

        await _estudiantes.AgregarAsync(estudiante);
        await _estudiantes.GuardarCambiosAsync();

        return Mapeador.AEstudianteDto(estudiante);
    }

    public async Task EliminarAsync(int id)
    {
        var estudiante = await _estudiantes.ObtenerPorIdAsync(id)
            ?? throw new RecursoNoEncontradoException("El estudiante no existe.");

        if (estudiante.GrupoExposicionId.HasValue)
        {
            throw new ReglaNegocioException("No se puede eliminar un estudiante que pertenece a un grupo de exposición.");
        }

        _estudiantes.Eliminar(estudiante);
        await _estudiantes.GuardarCambiosAsync();
    }
}
