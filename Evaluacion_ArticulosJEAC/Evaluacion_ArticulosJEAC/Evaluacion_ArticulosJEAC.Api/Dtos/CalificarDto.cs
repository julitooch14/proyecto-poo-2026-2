using System.ComponentModel.DataAnnotations;

namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class CalificarDto
{
    [Required(ErrorMessage = "La nota es obligatoria.")]
    [Range(typeof(decimal), "0", "5", ErrorMessage = "La nota debe estar entre 0.0 y 5.0.")]
    public decimal? Nota { get; set; }

    [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
    public string? Observaciones { get; set; }
}
