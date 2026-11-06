using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Api.Mappers;
using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Evaluacion_ArticulosJEAC.Api.Security;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public class DocenteServicio : IDocenteServicio
{
    private readonly IRepositorio<Docente> _docentes;
    private readonly IRepositorio<Usuario> _usuarios;
    private readonly IRepositorio<GrupoExposicion> _grupos;
    private readonly IRepositorio<Evaluacion> _evaluaciones;
    private readonly IHasheadorContrasenas _hasheador;

    public DocenteServicio(
        IRepositorio<Docente> docentes,
        IRepositorio<Usuario> usuarios,
        IRepositorio<GrupoExposicion> grupos,
        IRepositorio<Evaluacion> evaluaciones,
        IHasheadorContrasenas hasheador)
    {
        _docentes = docentes;
        _usuarios = usuarios;
        _grupos = grupos;
        _evaluaciones = evaluaciones;
        _hasheador = hasheador;
    }

    public async Task<List<DocenteDto>> ListarAsync()
    {
        var docentes = await _docentes.Consultar()
            .AsNoTracking()
            .Include(d => d.Usuario)
            .OrderBy(d => d.Nombre)
            .ToListAsync();

        return docentes.Select(d => Mapeador.ADocenteDto(d)).ToList();
    }

    public async Task<DocenteDto> CrearAsync(CrearDocenteDto dto)
    {
        var correo = dto.Correo.Trim().ToLowerInvariant();
        var nombreUsuario = dto.NombreUsuario.Trim();

        if (await _docentes.Consultar().AnyAsync(d => d.Correo == correo))
        {
            throw new ReglaNegocioException("Ya existe un docente con ese correo.");
        }

        if (await _usuarios.Consultar().AnyAsync(u => u.NombreUsuario == nombreUsuario))
        {
            throw new ReglaNegocioException("El usuario de acceso ya está en uso.");
        }

        var docente = new Docente
        {
            Nombre = dto.Nombre.Trim(),
            Correo = correo,
            Departamento = dto.Departamento.Trim(),
            Usuario = new Usuario
            {
                NombreUsuario = nombreUsuario,
                PasswordHash = _hasheador.Hashear(dto.Contrasena),
                Rol = RolUsuario.Docente
            }
        };

        await _docentes.AgregarAsync(docente);
        await _docentes.GuardarCambiosAsync();

        return Mapeador.ADocenteDto(docente);
    }

    public async Task EliminarAsync(int id)
    {
        var docente = await _docentes.Consultar()
            .Include(d => d.Usuario)
            .FirstOrDefaultAsync(d => d.Id == id)
            ?? throw new RecursoNoEncontradoException("El docente no existe.");

        var esAsesor = await _grupos.Consultar().AnyAsync(g => g.AsesorId == id);
        var esEvaluador = await _evaluaciones.Consultar().AnyAsync(e => e.DocenteEvaluadorId == id);

        if (esAsesor || esEvaluador)
        {
            throw new ReglaNegocioException("No se puede eliminar un docente que es asesor o evaluador de algún grupo.");
        }

        _docentes.Eliminar(docente);
        await _docentes.GuardarCambiosAsync();
    }
}
