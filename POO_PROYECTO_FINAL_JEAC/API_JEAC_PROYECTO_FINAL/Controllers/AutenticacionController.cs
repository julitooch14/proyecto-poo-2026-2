using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Modelos.DTO;
using Servicios.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_JEAC_PROYECTO_FINAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAutenticacion _autenticacion;

        public AutenticacionController(IConfiguration configuration, IAutenticacion autenticacion)
        {
            _configuration = configuration;
            _autenticacion = autenticacion;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var usuario = _autenticacion.ValidarCredenciales(request.Usuario, request.Contrasena);

            if (usuario is null)
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.NombreUsuario),
                new(ClaimTypes.Role, usuario.Rol)
            };

            // El token de un estudiante lleva su EstudianteId para que el servidor
            // sepa qué horario le corresponde sin confiar en nada que envíe el cliente.
            if (usuario.EstudianteId is int idEstudiante)
                claims.Add(new Claim("EstudianteId", idEstudiante.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(
                _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expira = expiration,
                rol = usuario.Rol,
                nombre = usuario.Nombre
            });
        }
    }
}
