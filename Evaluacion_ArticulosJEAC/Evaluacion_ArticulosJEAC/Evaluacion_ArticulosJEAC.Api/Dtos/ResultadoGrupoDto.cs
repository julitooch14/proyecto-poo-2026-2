namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class ResultadoGrupoDto
{
    public int GrupoId { get; set; }
    public string GrupoNombre { get; set; } = string.Empty;
    public string ArticuloTitulo { get; set; } = string.Empty;
    public int TotalEvaluadores { get; set; }
    public int Calificadas { get; set; }
    public decimal? Promedio { get; set; }
    public string Estado { get; set; } = string.Empty;
}
