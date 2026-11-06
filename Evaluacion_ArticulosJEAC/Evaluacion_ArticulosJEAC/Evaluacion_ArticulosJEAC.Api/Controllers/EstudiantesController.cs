using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion_ArticulosJEAC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class EstudiantesController : ControllerBase
{
    private readonly IEstudianteServicio _servicio;

    public EstudiantesController(IEstudianteServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<EstudianteDto>>> Listar()
    {
        return Ok(await _servicio.ListarAsync());
    }

    [HttpPost]
    public async Task<ActionResult<EstudianteDto>> Crear(CrearEstudianteDto dto)
    {
        var creado = await _servicio.CrearAsync(dto);
        return Created($"api/estudiantes/{creado.Id}", creado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}
