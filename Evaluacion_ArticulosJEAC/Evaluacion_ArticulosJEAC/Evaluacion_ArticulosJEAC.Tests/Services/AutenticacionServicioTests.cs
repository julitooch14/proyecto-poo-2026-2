using System.IdentityModel.Tokens.Jwt;
using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Evaluacion_ArticulosJEAC.Api.Security;
using Evaluacion_ArticulosJEAC.Api.Services;
using Evaluacion_ArticulosJEAC.Tests.Helpers;
using Microsoft.Extensions.Options;

namespace Evaluacion_ArticulosJEAC.Tests.Services;

public class AutenticacionServicioTests
{
    private readonly HasheadorContrasenas _hasheador = new();
    private readonly AutenticacionServicio _servicio;

    public AutenticacionServicioTests()
    {
        var contexto = FabricaContexto.Crear();
        var docente = new Docente { Nombre = "Docente Prueba", Correo = "docente@uco.edu.co", Departamento = "Sistemas" };

        contexto.Usuarios.Add(new Usuario
        {
            NombreUsuario = "user",
            PasswordHash = _hasheador.Hashear("user123"),
            Rol = RolUsuario.Docente,
            Docente = docente
        });
        contexto.Usuarios.Add(new Usuario
        {
            NombreUsuario = "admin",
            PasswordHash = _hasheador.Hashear("admin123"),
            Rol = RolUsuario.Admin
        });
        contexto.SaveChanges();

        var opciones = Options.Create(new OpcionesJwt
        {
            Clave = "clave-de-pruebas-con-longitud-suficiente-para-hmac-sha256",
            Emisor = "emisor-pruebas",
            Audiencia = "audiencia-pruebas",
            MinutosExpiracion = 30
        });

        _servicio = new AutenticacionServicio(
            new Repositorio<Usuario>(contexto),
            _hasheador,
            new GeneradorToken(opciones));
    }

    [Fact]
    public async Task Autenticar_ConCredencialesDeDocente_DevuelveTokenYRolDocente()
    {
        var respuesta = await _servicio.AutenticarAsync(new SolicitudLoginDto { NombreUsuario = "user", Contrasena = "user123" });

        Assert.Equal("Docente", respuesta.Rol);
        Assert.NotNull(respuesta.DocenteId);
        Assert.Equal("Docente Prueba", respuesta.NombreDocente);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(respuesta.Token);
        Assert.Contains(token.Claims, c => c.Value == "Docente");
        Assert.Equal("emisor-pruebas", token.Issuer);
    }

    [Fact]
    public async Task Autenticar_ConCredencialesDeAdmin_DevuelveRolAdminSinDocente()
    {
        var respuesta = await _servicio.AutenticarAsync(new SolicitudLoginDto { NombreUsuario = "admin", Contrasena = "admin123" });

        Assert.Equal("Admin", respuesta.Rol);
        Assert.Null(respuesta.DocenteId);
    }

    [Fact]
    public async Task Autenticar_ConContrasenaIncorrecta_LanzaCredencialesInvalidas()
    {
        await Assert.ThrowsAsync<CredencialesInvalidasException>(() =>
            _servicio.AutenticarAsync(new SolicitudLoginDto { NombreUsuario = "user", Contrasena = "incorrecta" }));
    }

    [Fact]
    public async Task Autenticar_ConUsuarioInexistente_LanzaCredencialesInvalidas()
    {
        await Assert.ThrowsAsync<CredencialesInvalidasException>(() =>
            _servicio.AutenticarAsync(new SolicitudLoginDto { NombreUsuario = "nadie", Contrasena = "x" }));
    }
}
