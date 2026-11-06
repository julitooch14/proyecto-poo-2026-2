using Evaluacion_ArticulosJEAC.Api.Security;

namespace Evaluacion_ArticulosJEAC.Tests.Security;

public class HasheadorContrasenasTests
{
    private readonly HasheadorContrasenas _hasheador = new();

    [Fact]
    public void Verificar_ConLaContrasenaCorrecta_DevuelveVerdadero()
    {
        var hash = _hasheador.Hashear("secreta123");

        Assert.True(_hasheador.Verificar("secreta123", hash));
    }

    [Fact]
    public void Verificar_ConUnaContrasenaDistinta_DevuelveFalso()
    {
        var hash = _hasheador.Hashear("secreta123");

        Assert.False(_hasheador.Verificar("otra", hash));
    }

    [Fact]
    public void Hashear_LaMismaContrasenaDosVeces_GeneraHashesDistintos()
    {
        Assert.NotEqual(_hasheador.Hashear("secreta123"), _hasheador.Hashear("secreta123"));
    }

    [Fact]
    public void Hashear_NoAlmacenaLaContrasenaEnTextoPlano()
    {
        Assert.DoesNotContain("secreta123", _hasheador.Hashear("secreta123"));
    }

    [Fact]
    public void Verificar_ConUnHashMalFormado_DevuelveFalso()
    {
        Assert.False(_hasheador.Verificar("secreta123", "no-es-un-hash"));
    }
}
