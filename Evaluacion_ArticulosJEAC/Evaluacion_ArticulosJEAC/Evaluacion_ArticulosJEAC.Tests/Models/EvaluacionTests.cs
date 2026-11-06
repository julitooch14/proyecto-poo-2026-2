using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Api.Models;

namespace Evaluacion_ArticulosJEAC.Tests.Models;

public class EvaluacionTests
{
    [Fact]
    public void NuevaEvaluacion_NoEstaCalificada()
    {
        var evaluacion = new Evaluacion();

        Assert.False(evaluacion.EstaCalificada);
        Assert.Null(evaluacion.Nota);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2.5)]
    [InlineData(5)]
    public void Calificar_ConNotaValida_GuardaLaNota(double nota)
    {
        var evaluacion = new Evaluacion();

        evaluacion.Calificar((decimal)nota, "Observación");

        Assert.True(evaluacion.EstaCalificada);
        Assert.Equal((decimal)nota, evaluacion.Nota);
        Assert.Equal("Observación", evaluacion.Observaciones);
        Assert.NotNull(evaluacion.FechaCalificacion);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(5.1)]
    [InlineData(10)]
    public void Calificar_ConNotaFueraDeRango_LanzaReglaNegocio(double nota)
    {
        var evaluacion = new Evaluacion();

        Assert.Throws<ReglaNegocioException>(() => evaluacion.Calificar((decimal)nota, null));
        Assert.False(evaluacion.EstaCalificada);
    }

    [Fact]
    public void Calificar_RedondeaAUnDecimal()
    {
        var evaluacion = new Evaluacion();

        evaluacion.Calificar(4.26m, null);

        Assert.Equal(4.3m, evaluacion.Nota);
    }

    [Fact]
    public void Calificar_ConObservacionVacia_GuardaNulo()
    {
        var evaluacion = new Evaluacion();

        evaluacion.Calificar(3m, "   ");

        Assert.Null(evaluacion.Observaciones);
    }
}
