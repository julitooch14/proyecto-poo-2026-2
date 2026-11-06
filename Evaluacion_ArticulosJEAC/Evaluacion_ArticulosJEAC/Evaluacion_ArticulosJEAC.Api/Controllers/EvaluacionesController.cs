using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Security;
using Evaluacion_ArticulosJEAC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion_ArticulosJEAC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EvaluacionesController : ControllerBase
{
    private readonly IEvaluacionServicio _servicio;

    public EvaluacionesController(IEvaluacionServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpGet("mis-evaluaciones")]
    [Authorize(Roles = "Docente")]
    public async Task<ActionResult<List<EvaluacionDto>>> MisEvaluaciones()
    {
        return Ok(await _servicio.ListarPorDocenteAsync(User.ObtenerDocenteId()));
    }

    [HttpPut("{id:int}/calificar")]
    [Authorize(Roles = "Docente")]
    public async Task<ActionResult<EvaluacionDto>> Calificar(int id, CalificarDto dto)
    {
        return Ok(await _servicio.CalificarAsync(id, User.ObtenerDocenteId(), dto));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<EvaluacionDto>>> Listar()
    {
        return Ok(await _servicio.ListarTodasAsync());
    }

    [HttpGet("resultados")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ResultadoGrupoDto>>> Resultados()
    {
        return Ok(await _servicio.ObtenerResultadosAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EvaluacionDto>> Asignar(AsignarEvaluadorDto dto)
    {
        var creada = await _servicio.AsignarEvaluadorAsync(dto);
        return Created($"api/evaluaciones/{creada.Id}", creada);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsignacionAsync(id);
        return NoContent();
    }
}
