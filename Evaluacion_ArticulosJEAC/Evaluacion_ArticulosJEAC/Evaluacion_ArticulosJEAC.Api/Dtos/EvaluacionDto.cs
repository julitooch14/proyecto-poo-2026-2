namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class EvaluacionDto
{
    public int Id { get; set; }
    public int GrupoId { get; set; }
    public string GrupoNombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    public DateTime? FechaExposicion { get; set; }
    public string ArticuloTitulo { get; set; } = string.Empty;
    public string ArticuloAutores { get; set; } = string.Empty;
    public string ArticuloResumen { get; set; } = string.Empty;
    public string AsesorNombre { get; set; } = string.Empty;
    public List<string> Estudiantes { get; set; } = new();
    public int DocenteEvaluadorId { get; set; }
    public string EvaluadorNombre { get; set; } = string.Empty;
    public decimal? Nota { get; set; }
    public string? Observaciones { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaCalificacion { get; set; }
}
