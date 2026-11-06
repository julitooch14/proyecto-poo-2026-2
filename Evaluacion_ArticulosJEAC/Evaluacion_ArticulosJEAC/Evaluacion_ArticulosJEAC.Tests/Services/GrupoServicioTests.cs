using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Evaluacion_ArticulosJEAC.Api.Services;
using Evaluacion_ArticulosJEAC.Tests.Helpers;

namespace Evaluacion_ArticulosJEAC.Tests.Services;

public class GrupoServicioTests
{
    [Fact]
    public async Task Crear_ConArticuloYaAsignado_LanzaReglaNegocio()
    {
        var escenario = new EscenarioEvaluacion();
        var servicio = CrearServicio(escenario);

        await Assert.ThrowsAsync<ReglaNegocioException>(() => servicio.CrearAsync(new CrearGrupoDto
        {
            Nombre = "Otro grupo",
            ArticuloId = escenario.Grupo.ArticuloCientificoId,
            AsesorId = escenario.Asesor.Id,
            Orden = 2
        }));
    }

    [Fact]
    public async Task Crear_ConEstudiantesLibres_LosAsignaAlGrupo()
    {
        var escenario = new EscenarioEvaluacion();
        var servicio = CrearServicio(escenario);
        var articulo = new ArticuloCientifico { Titulo = "Nuevo", Autores = "A", Revista = "R", Anio = 2024, Resumen = "Resumen" };
        var estudiante = new Estudiante { Nombre = "Estudiante Libre", Codigo = "EST-100" };
        escenario.Contexto.Articulos.Add(articulo);
        escenario.Contexto.Estudiantes.Add(estudiante);
        await escenario.Contexto.SaveChangesAsync();

        var grupo = await servicio.CrearAsync(new CrearGrupoDto
        {
            Nombre = "Grupo nuevo",
            ArticuloId = articulo.Id,
            AsesorId = escenario.Asesor.Id,
            Orden = 2,
            EstudianteIds = new List<int> { estudiante.Id }
        });

        Assert.Contains("Estudiante Libre", grupo.Estudiantes);
    }

    [Fact]
    public async Task Crear_ConEstudianteQueYaTieneGrupo_LanzaReglaNegocio()
    {
        var escenario = new EscenarioEvaluacion();
        var servicio = CrearServicio(escenario);
        var articulo = new ArticuloCientifico { Titulo = "Nuevo", Autores = "A", Revista = "R", Anio = 2024, Resumen = "Resumen" };
        var estudiante = new Estudiante { Nombre = "Ocupado", Codigo = "EST-101", GrupoExposicionId = escenario.Grupo.Id };
        escenario.Contexto.Articulos.Add(articulo);
        escenario.Contexto.Estudiantes.Add(estudiante);
        await escenario.Contexto.SaveChangesAsync();

        await Assert.ThrowsAsync<ReglaNegocioException>(() => servicio.CrearAsync(new CrearGrupoDto
        {
            Nombre = "Grupo nuevo",
            ArticuloId = articulo.Id,
            AsesorId = escenario.Asesor.Id,
            Orden = 2,
            EstudianteIds = new List<int> { estudiante.Id }
        }));
    }

    [Fact]
    public async Task Eliminar_GrupoConEvaluadores_LanzaReglaNegocio()
    {
        var escenario = new EscenarioEvaluacion();
        escenario.AgregarEvaluacion(escenario.EvaluadorA);
        var servicio = CrearServicio(escenario);

        await Assert.ThrowsAsync<ReglaNegocioException>(() => servicio.EliminarAsync(escenario.Grupo.Id));
    }

    private static GrupoServicio CrearServicio(EscenarioEvaluacion escenario)
    {
        return new GrupoServicio(
            new Repositorio<GrupoExposicion>(escenario.Contexto),
            new Repositorio<ArticuloCientifico>(escenario.Contexto),
            new Repositorio<Docente>(escenario.Contexto),
            new Repositorio<Estudiante>(escenario.Contexto));
    }
}
