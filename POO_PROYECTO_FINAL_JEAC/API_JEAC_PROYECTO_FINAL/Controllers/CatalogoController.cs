using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicios.Interfaces;

namespace API_JEAC_PROYECTO_FINAL.Controllers
{
    // Listas que el administrador necesita para armar un bloque:
    // qué grupos existen y qué docentes puede asignar.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogo _catalogo;

        public CatalogoController(ICatalogo catalogo)
        {
            _catalogo = catalogo;
        }

        [HttpGet("Grupos")]
        public IActionResult ObtenerGrupos()
        {
            return Ok(_catalogo.ObtenerGrupos());
        }

        [HttpGet("Evaluadores")]
        public IActionResult ObtenerEvaluadores()
        {
            return Ok(_catalogo.ObtenerEvaluadores());
        }
    }
}
