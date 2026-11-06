using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Models;

namespace Evaluacion_ArticulosJEAC.Api.Mappers;

public static class Mapeador
{
    public static DocenteDto ADocenteDto(Docente docente)
    {
        return new DocenteDto
        {
            Id = docente.Id,
            Nombre = docente.Nombre,
            Correo = docente.Correo,
            Departamento = docente.Departamento,
            NombreUsuario = docente.Usuario?.NombreUsuario
        };
    }

    public static EstudianteDto AEstudianteDto(Estudiante estudiante)
    {
        return new EstudianteDto
        {
            Id = estudiante.Id,
            Nombre = estudiante.Nombre,
            Codigo = estudiante.Codigo,
            GrupoId = estudiante.GrupoExposicionId,
            GrupoNombre = estudiante.Grupo?.Nombre
        };
    }

    public static ArticuloDto AArticuloDto(ArticuloCientifico articulo)
    {
        return new ArticuloDto
        {
            Id = articulo.Id,
            Titulo = articulo.Titulo,
            Autores = articulo.Autores,
            Revista = articulo.Revista,
            Anio = articulo.Anio,
            Resumen = articulo.Resumen
        };
    }

    public static GrupoDto AGrupoDto(GrupoExposicion grupo)
    {
        return new GrupoDto
        {
            Id = grupo.Id,
            Nombre = grupo.Nombre,
            ArticuloId = grupo.ArticuloCientificoId,
            ArticuloTitulo = grupo.Articulo.Titulo,
            AsesorId = grupo.AsesorId,
            AsesorNombre = grupo.Asesor.Nombre,
            Orden = grupo.Orden,
            FechaExposicion = grupo.FechaExposicion,
            Estudiantes = grupo.Estudiantes.OrderBy(e => e.Nombre).Select(e => e.Nombre).ToList(),
            TotalEvaluadores = grupo.Evaluaciones.Count
        };
    }

    public static EvaluacionDto AEvaluacionDto(Evaluacion evaluacion)
    {
        return new EvaluacionDto
        {
            Id = evaluacion.Id,
            GrupoId = evaluacion.GrupoExposicionId,
            GrupoNombre = evaluacion.Grupo.Nombre,
            Orden = evaluacion.Grupo.Orden,
            FechaExposicion = evaluacion.Grupo.FechaExposicion,
            ArticuloTitulo = evaluacion.Grupo.Articulo.Titulo,
            ArticuloAutores = evaluacion.Grupo.Articulo.Autores,
            ArticuloResumen = evaluacion.Grupo.Articulo.Resumen,
            AsesorNombre = evaluacion.Grupo.Asesor.Nombre,
            Estudiantes = evaluacion.Grupo.Estudiantes.OrderBy(e => e.Nombre).Select(e => e.Nombre).ToList(),
            DocenteEvaluadorId = evaluacion.DocenteEvaluadorId,
            EvaluadorNombre = evaluacion.Evaluador.Nombre,
            Nota = evaluacion.Nota,
            Observaciones = evaluacion.Observaciones,
            Estado = evaluacion.EstaCalificada ? "Calificada" : "Pendiente",
            FechaCalificacion = evaluacion.FechaCalificacion
        };
    }
}
