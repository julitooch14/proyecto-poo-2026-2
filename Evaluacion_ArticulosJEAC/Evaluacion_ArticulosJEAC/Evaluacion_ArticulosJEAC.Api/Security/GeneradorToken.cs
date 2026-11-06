using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Evaluacion_ArticulosJEAC.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Evaluacion_ArticulosJEAC.Api.Security;

public class GeneradorToken : IGeneradorToken
{
    private readonly OpcionesJwt _opciones;

    public GeneradorToken(IOptions<OpcionesJwt> opciones)
    {
        _opciones = opciones.Value;
    }

    public TokenGenerado Generar(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreUsuario),
            new(ClaimTypes.Role, usuario.Rol.ToString())
        };

        if (usuario.DocenteId.HasValue)
        {
            claims.Add(new Claim("docenteId", usuario.DocenteId.Value.ToString()));
        }

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.Clave));
        var expira = DateTime.UtcNow.AddMinutes(_opciones.MinutosExpiracion);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expira,
            Issuer = _opciones.Emisor,
            Audience = _opciones.Audiencia,
            SigningCredentials = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256)
        };

        var manejador = new JwtSecurityTokenHandler();
        var token = manejador.CreateToken(descriptor);
        return new TokenGenerado(manejador.WriteToken(token), expira);
    }
}
