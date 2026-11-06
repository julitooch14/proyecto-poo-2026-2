using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Evaluacion_ArticulosJEAC.Api.Controllers;

[ApiController]
[Route("api/autenticacion")]
[AllowAnonymous]
public class AutenticacionController : ControllerBase
{
    private readonly IAutenticacionServicio _servicio;

    public AutenticacionController(IAutenticacionServicio servicio)
    {
        _servicio = servicio;
    }

    [HttpPost("login")]
    public async Task<ActionResult<RespuestaLoginDto>> Login(SolicitudLoginDto solicitud)
    {
        var respuesta = await _servicio.AutenticarAsync(solicitud);
        return Ok(respuesta);
    }
}
