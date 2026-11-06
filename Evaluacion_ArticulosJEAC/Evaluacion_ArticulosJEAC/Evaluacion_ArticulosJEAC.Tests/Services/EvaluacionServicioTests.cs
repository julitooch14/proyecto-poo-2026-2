using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Tests.Helpers;

namespace Evaluacion_ArticulosJEAC.Tests.Services;

public class EvaluacionServicioTests
{
    [Fact]
    public async Task AsignarEvaluador_Valido_CreaEvaluacionPendiente()
    {
        var escenario = new EscenarioEvaluacion();

        var resultado = await escenario.Servicio.AsignarEvaluadorAsync(new AsignarEvaluadorDto
        {
            GrupoId = escenario.Grupo.Id,
            DocenteId = escenario.EvaluadorA.Id
        });

        Assert.Equal("Pendiente", resultado.Estado);
        Assert.Null(resultado.Nota);
        Assert.Equal(escenario.EvaluadorA.Nombre, resultado.EvaluadorNombre);
    }

    [Fact]
    public async Task AsignarEvaluador_ConElAsesorDelGrupo_LanzaReglaNegocio()
    {
        var escenario = new EscenarioEvaluacion();

        await Assert.ThrowsAsync<ReglaNegocioException>(() =>
            escenario.Servicio.AsignarEvaluadorAsync(new AsignarEvaluadorDto
            {
                GrupoId = escenario.Grupo.Id,
                DocenteId = escenario.Asesor.Id
            }));
    }

    [Fact]
    public async Task AsignarEvaluador_Duplicado_LanzaReglaNegocio()
    {
        var escenario = new EscenarioEvaluacion();
        escenario.AgregarEvaluacion(escenario.EvaluadorA);

        await Assert.ThrowsAsync<ReglaNegocioException>(() =>
            escenario.Servicio.AsignarEvaluadorAsync(new AsignarEvaluadorDto
            {
                GrupoId = escenario.Grupo.Id,
                DocenteId = escenario.EvaluadorA.Id
            }));
    }

    [Fact]
    public async Task AsignarEvaluador_GrupoInexistente_LanzaNoEncontrado()
    {
        var escenario = new EscenarioEvaluacion();

        await Assert.ThrowsAsync<RecursoNoEncontradoException>(() =>
            escenario.Servicio.AsignarEvaluadorAsync(new AsignarEvaluadorDto
            {
                GrupoId = 999,
                DocenteId = escenario.EvaluadorA.Id
            }));
    }

    [Fact]
    public async Task Calificar_PorElDocenteAsignado_GuardaLaNota()
    {
        var escenario = new EscenarioEvaluacion();
        var evaluacion = escenario.AgregarEvaluacion(escenario.EvaluadorA);

        var resultado = await escenario.Servicio.CalificarAsync(
            evaluacion.Id,
            escenario.EvaluadorA.Id,
            new CalificarDto { Nota = 4.2m, Observaciones = "Muy bien" });

        Assert.Equal("Calificada", resultado.Estado);
        Assert.Equal(4.2m, resultado.Nota);
        Assert.Equal("Muy bien", resultado.Observaciones);
    }

    [Fact]
    public async Task Calificar_PorOtroDocente_LanzaAccesoDenegado()
    {
        var escenario = new EscenarioEvaluacion();
        var evaluacion = escenario.AgregarEvaluacion(escenario.EvaluadorA);

        await Assert.ThrowsAsync<AccesoDenegadoException>(() =>
            escenario.Servicio.CalificarAsync(
                evaluacion.Id,
                escenario.EvaluadorB.Id,
                new CalificarDto { Nota = 3m }));
    }

    [Fact]
    public async Task Calificar_EvaluacionInexistente_LanzaNoEncontrado()
    {
        var escenario = new EscenarioEvaluacion();

        await Assert.ThrowsAsync<RecursoNoEncontradoException>(() =>
            escenario.Servicio.CalificarAsync(999, escenario.EvaluadorA.Id, new CalificarDto { Nota = 3m }));
    }

    [Fact]
    public async Task Calificar_ConNotaFueraDeRango_LanzaReglaNegocio()
    {
        var escenario = new EscenarioEvaluacion();
        var evaluacion = escenario.AgregarEvaluacion(escenario.EvaluadorA);

        await Assert.ThrowsAsync<ReglaNegocioException>(() =>
            escenario.Servicio.CalificarAsync(
                evaluacion.Id,
                escenario.EvaluadorA.Id,
                new CalificarDto { Nota = 6m }));
    }

    [Fact]
    public async Task ListarPorDocente_SoloDevuelveLasEvaluacionesPropias()
    {
        var escenario = new EscenarioEvaluacion();
        escenario.AgregarEvaluacion(escenario.EvaluadorA);
        escenario.AgregarEvaluacion(escenario.EvaluadorB);

        var resultado = await escenario.Servicio.ListarPorDocenteAsync(escenario.EvaluadorA.Id);

        Assert.Single(resultado);
        Assert.Equal(escenario.EvaluadorA.Id, resultado[0].DocenteEvaluadorId);
    }

    [Fact]
    public async Task EliminarAsignacion_Calificada_LanzaReglaNegocio()
    {
        var escenario = new EscenarioEvaluacion();
        var evaluacion = escenario.AgregarEvaluacion(escenario.EvaluadorA, 4m);

        await Assert.ThrowsAsync<ReglaNegocioException>(() =>
            escenario.Servicio.EliminarAsignacionAsync(evaluacion.Id));
    }

    [Fact]
    public async Task EliminarAsignacion_Pendiente_LaElimina()
    {
        var escenario = new EscenarioEvaluacion();
        var evaluacion = escenario.AgregarEvaluacion(escenario.EvaluadorA);

        await escenario.Servicio.EliminarAsignacionAsync(evaluacion.Id);

        Assert.Empty(await escenario.Servicio.ListarTodasAsync());
    }

    [Fact]
    public async Task ObtenerResultados_ConDosNotas_CalculaElPromedioYConsolida()
    {
        var escenario = new EscenarioEvaluacion();
        escenario.AgregarEvaluacion(escenario.EvaluadorA, 4m);
        escenario.AgregarEvaluacion(escenario.EvaluadorB, 5m);

        var resultados = await escenario.Servicio.ObtenerResultadosAsync();

        var resultado = Assert.Single(resultados);
        Assert.Equal(4.5m, resultado.Promedio);
        Assert.Equal(2, resultado.Calificadas);
        Assert.Equal("Consolidado", resultado.Estado);
    }

    [Fact]
    public async Task ObtenerResultados_ConUnaNotaPendiente_EstaEnEvaluacion()
    {
        var escenario = new EscenarioEvaluacion();
        escenario.AgregarEvaluacion(escenario.EvaluadorA, 4m);
        escenario.AgregarEvaluacion(escenario.EvaluadorB);

        var resultado = Assert.Single(await escenario.Servicio.ObtenerResultadosAsync());

        Assert.Equal("En evaluación", resultado.Estado);
        Assert.Equal(1, resultado.Calificadas);
    }

    [Fact]
    public async Task ObtenerResultados_ConMenosDelMinimoDeEvaluadores_IndicaQueFaltan()
    {
        var escenario = new EscenarioEvaluacion();
        escenario.AgregarEvaluacion(escenario.EvaluadorA, 4m);

        var resultado = Assert.Single(await escenario.Servicio.ObtenerResultadosAsync());

        Assert.Equal("Faltan evaluadores", resultado.Estado);
    }
}
