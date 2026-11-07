using Microsoft.AspNetCore.Mvc;
using jeac.domain.modelos.entidades;
using jeac.services;

namespace final_de_poo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthServicio _authServicio;
        private static readonly ProcesoServicio _procesoServicio = new ProcesoServicio();

        public AuthController(AuthServicio authServicio)
        {
            _authServicio = authServicio;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UsuarioLoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || loginDto.Password != "123456")
            {
                return Unauthorized(new { Mensaje = "Credenciales incorrectas" });
            }

            // Administrador
            if (loginDto.Email == "admin@uco.edu.co")
            {
                var token = _authServicio.GenerarToken(loginDto.Email, "Administrador");
                return Ok(new { Token = token, Rol = "Administrador" });
            }

            // Docente
            if (loginDto.Email == "jorge.mario@uco.edu.co" || loginDto.Email == "ricardo.cogollo@uco.edu.co")
            {
                var token = _authServicio.GenerarToken(loginDto.Email, "Docente");
                return Ok(new { Token = token, Rol = "Docente" });
            }

            // Estudiante
            var grupoEstudiante = _procesoServicio.ConsultarGrupoPorEstudiante(loginDto.Email);
            if (grupoEstudiante != null)
            {
                var token = _authServicio.GenerarToken(loginDto.Email, "Estudiante");
                return Ok(new { Token = token, Rol = "Estudiante" });
            }

            return Unauthorized(new { Mensaje = "El correo de estudiante no está registrado en ningún grupo." });
        }
    }
}