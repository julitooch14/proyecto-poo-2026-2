using System.ComponentModel.DataAnnotations;

namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class AsignarEvaluadorDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un grupo.")]
    public int GrupoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un docente.")]
    public int DocenteId { get; set; }
}
