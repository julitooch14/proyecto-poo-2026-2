using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Evaluacion_ArticulosJEAC.Api.Security;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public class AutenticacionServicio : IAutenticacionServicio
{
    private readonly IRepositorio<Usuario> _usuarios;
    private readonly IHasheadorContrasenas _hasheador;
    private readonly IGeneradorToken _generadorToken;

    public AutenticacionServicio(
        IRepositorio<Usuario> usuarios,
        IHasheadorContrasenas hasheador,
        IGeneradorToken generadorToken)
    {
        _usuarios = usuarios;
        _hasheador = hasheador;
        _generadorToken = generadorToken;
    }

    public async Task<RespuestaLoginDto> AutenticarAsync(SolicitudLoginDto solicitud)
    {
        var nombreUsuario = solicitud.NombreUsuario.Trim();

        var usuario = await _usuarios.Consultar()
            .Include(u => u.Docente)
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

        if (usuario is null || !_hasheador.Verificar(solicitud.Contrasena, usuario.PasswordHash))
        {
            throw new CredencialesInvalidasException();
        }

        var token = _generadorToken.Generar(usuario);

        return new RespuestaLoginDto
        {
            Token = token.Token,
            Expira = token.Expira,
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol.ToString(),
            DocenteId = usuario.DocenteId,
            NombreDocente = usuario.Docente?.Nombre
        };
    }
}
