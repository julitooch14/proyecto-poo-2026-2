using System.ComponentModel.DataAnnotations;

namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class CrearGrupoDto
{
    [Required(ErrorMessage = "El nombre del grupo es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un artículo.")]
    public int ArticuloId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un asesor.")]
    public int AsesorId { get; set; }

    [Range(1, 1000, ErrorMessage = "El orden debe ser un número positivo.")]
    public int Orden { get; set; }

    public DateTime? FechaExposicion { get; set; }

    public List<int> EstudianteIds { get; set; } = new();
}
