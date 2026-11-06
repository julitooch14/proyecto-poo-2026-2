using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion_ArticulosJEAC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ArticulosController : ControllerBase
{
    private readonly IArticuloServicio _servicio;

    public ArticulosController(IArticuloServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<ArticuloDto>>> Listar()
    {
        return Ok(await _servicio.ListarAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ArticuloDto>> Crear(CrearArticuloDto dto)
    {
        var creado = await _servicio.CrearAsync(dto);
        return Created($"api/articulos/{creado.Id}", creado);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return NoContent();
    }
}
