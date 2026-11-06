using Evaluacion_ArticulosJEAC.Api.Exceptions;

namespace Evaluacion_ArticulosJEAC.Api.Middleware;

public class ManejadorErroresMiddleware
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ManejadorErroresMiddleware> _logger;

    public ManejadorErroresMiddleware(RequestDelegate siguiente, ILogger<ManejadorErroresMiddleware> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (Exception excepcion)
        {
            await ManejarAsync(contexto, excepcion);
        }
    }

    private async Task ManejarAsync(HttpContext contexto, Exception excepcion)
    {
        var (codigo, mensaje) = excepcion switch
        {
            ReglaNegocioException => (StatusCodes.Status400BadRequest, excepcion.Message),
            RecursoNoEncontradoException => (StatusCodes.Status404NotFound, excepcion.Message),
            CredencialesInvalidasException => (StatusCodes.Status401Unauthorized, excepcion.Message),
            AccesoDenegadoException => (StatusCodes.Status403Forbidden, excepcion.Message),
            _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado en el servidor.")
        };

        if (codigo == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(excepcion, "Error no controlado en la solicitud {Ruta}", contexto.Request.Path);
        }

        contexto.Response.StatusCode = codigo;
        await contexto.Response.WriteAsJsonAsync(new { mensaje });
    }
}
