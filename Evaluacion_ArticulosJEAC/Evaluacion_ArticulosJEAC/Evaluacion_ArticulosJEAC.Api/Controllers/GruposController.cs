using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion_ArticulosJEAC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class GruposController : ControllerBase
{
    private readonly IGrupoServicio _servicio;

    public GruposController(IGrupoServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<GrupoDto>>> Listar()
    {
        return Ok(await _servicio.ListarAsync());
    }

    [HttpPost]
    public async Task<ActionResult<GrupoDto>> Crear(CrearGrupoDto dto)
    {
        var creado = await _servicio.CrearAsync(dto);
        return Created($"api/grupos/{creado.Id}", creado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}
