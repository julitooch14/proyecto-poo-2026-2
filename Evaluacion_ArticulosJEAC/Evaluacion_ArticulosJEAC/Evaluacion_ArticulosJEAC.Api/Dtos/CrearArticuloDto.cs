using System.ComponentModel.DataAnnotations;

namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class CrearArticuloDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(250)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los autores son obligatorios.")]
    [StringLength(250)]
    public string Autores { get; set; } = string.Empty;

    [Required(ErrorMessage = "La revista es obligatoria.")]
    [StringLength(150)]
    public string Revista { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "El año debe estar entre 1900 y 2100.")]
    public int Anio { get; set; }

    [Required(ErrorMessage = "El resumen es obligatorio.")]
    [StringLength(2000)]
    public string Resumen { get; set; } = string.Empty;
}
