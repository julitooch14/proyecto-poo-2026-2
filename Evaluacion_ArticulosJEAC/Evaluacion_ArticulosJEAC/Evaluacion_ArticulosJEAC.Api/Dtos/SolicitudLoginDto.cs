using System.ComponentModel.DataAnnotations;

namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class SolicitudLoginDto
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Contrasena { get; set; } = string.Empty;
}
