using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using jeac.domain.modelos.entidades;
using jeac.services;

namespace final_de_poo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcesoController : ControllerBase
    {
        private static readonly ProcesoServicio _servicio = new ProcesoServicio();

        public class AsignacionManualDto
        {
            public int GrupoId { get; set; }
            public int ArticuloId { get; set; }
        }

        [HttpPost("docente/asignar-manual")]
        [Authorize(Roles = "Docente")]
        public IActionResult AsignarManualmente([FromBody] AsignacionManualDto dto)
        {
            var docenteEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(docenteEmail)) return Unauthorized();

            bool exito = _servicio.AsignarArticuloManualmente(dto.GrupoId, dto.ArticuloId, docenteEmail);

            if (exito)
            {
                if (dto.ArticuloId == 0)
                    return Ok(new { Mensaje = $"Se ha liberado la asignación de artículo para el Grupo #{dto.GrupoId}." });

                return Ok(new { Mensaje = $"Artículo #{dto.ArticuloId} asignado exclusivamente al Grupo #{dto.GrupoId}." });
            }

            return BadRequest(new { Mensaje = "No se pudo asignar. El artículo ya fue asignado exclusivamente a otro grupo o no tienes permiso sobre este grupo." });
        }

        [HttpGet("docente/mis-grupos")]
        [Authorize(Roles = "Docente")]
        public IActionResult MisGruposDocente()
        {
            var docenteEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok(_servicio.ConsultarGruposPorDocente(docenteEmail!));
        }

        [HttpGet("estudiante/mi-grupo")]
        [Authorize(Roles = "Estudiante")]
        public IActionResult MiGrupoEstudiante()
        {
            var estudianteEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            var grupo = _servicio.ConsultarGrupoPorEstudiante(estudianteEmail!);
            if (grupo == null) return NotFound(new { Mensaje = "No estás registrado en ningún grupo." });

            return Ok(grupo);
        }

        [HttpGet("admin/consolidado")]
        [Authorize(Roles = "Administrador")]
        public IActionResult VisualizarAdmin()
        {
            return Ok(_servicio.ConsultarTodosLosGrupos());
        }

        [HttpGet("articulos")]
        [Authorize]
        public IActionResult ObtenerArticulos()
        {
            return Ok(_servicio.ConsultarArticulos());
        }
    }
}