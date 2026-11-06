using Microsoft.AspNetCore.Mvc;
using final_de_poo.modelos.entidades;
using final_de_poo.servicios;

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

            // 1. Rol Administrador
            if (loginDto.Email == "admin@uco.edu.co")
            {
                var token = _authServicio.GenerarToken(loginDto.Email, "Administrador");
                return Ok(new { Token = token, Rol = "Administrador" });
            }

            // 2. Rol Docentes
            if (loginDto.Email == "jorge.mario@uco.edu.co" || loginDto.Email == "ricardo.cogollo@uco.edu.co")
            {
                var token = _authServicio.GenerarToken(loginDto.Email, "Docente");
                return Ok(new { Token = token, Rol = "Docente" });
            }

            // 3. Rol Estudiantes (Verifica si el correo existe dentro de los grupos)
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