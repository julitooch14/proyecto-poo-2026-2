using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modelos.DTO;
using Servicios.Interfaces;
using System.Security.Claims;

namespace API_JEAC_PROYECTO_FINAL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BloquesController : ControllerBase
    {
        private readonly IBloqueExposicion _bloques;

        public BloquesController(IBloqueExposicion bloques)
        {
            _bloques = bloques;
        }

        // Docentes y administrador ven todos los bloques.
        [HttpGet]
        [Authorize(Roles = "Docente,Administrador")]
        public IActionResult Listar()
        {
            return Ok(_bloques.Listar());
        }

        // El estudiante solo ve el horario de SU grupo. El servidor lo deduce del token
        // y filtra los horarios, así que no puede consultar los de otros grupos.
        [HttpGet("MiHorario")]
        [Authorize(Roles = "Estudiante")]
        public IActionResult MiHorario()
        {
            if (!int.TryParse(User.FindFirstValue("EstudianteId"), out var idEstudiante))
                return Unauthorized(new { mensaje = "El token no contiene un estudiante válido." });

            return this.ARespuesta(_bloques.ObtenerMiHorario(idEstudiante));
        }

        // Guardar un bloque completo (salón, docentes y todos sus horarios) en una sola operación.
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult Crear([FromBody] BloqueRequest solicitud)
        {
            return this.ARespuesta(_bloques.Crear(solicitud));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Actualizar(int id, [FromBody] BloqueRequest solicitud)
        {
            return this.ARespuesta(_bloques.Actualizar(id, solicitud));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var resultado = _bloques.Eliminar(id);
            if (!resultado.Exitoso)
                return this.ARespuesta(resultado);

            return Ok(new { mensaje = resultado.Valor });
        }
    }
}
