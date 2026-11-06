using Microsoft.AspNetCore.Mvc;
using Modelos.Resultados;

namespace API_JEAC_PROYECTO_FINAL.Controllers
{
    // Traduce lo que devuelve un servicio (Resultado) a la respuesta HTTP correspondiente.
    public static class ResultadoExtensions
    {
        public static IActionResult ARespuesta<T>(this ControllerBase controlador, Resultado<T> resultado)
        {
            if (resultado.Exitoso)
                return controlador.Ok(resultado.Valor);

            var cuerpo = new { mensaje = resultado.Mensaje };

            return resultado.Tipo switch
            {
                TipoResultado.SolicitudInvalida => controlador.BadRequest(cuerpo),
                TipoResultado.NoEncontrado => controlador.NotFound(cuerpo),
                TipoResultado.Conflicto => controlador.Conflict(cuerpo),
                TipoResultado.NoAutorizado => controlador.Unauthorized(cuerpo),
                _ => controlador.StatusCode(500, cuerpo)
            };
        }
    }
}
