namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class GrupoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int ArticuloId { get; set; }
    public string ArticuloTitulo { get; set; } = string.Empty;
    public int AsesorId { get; set; }
    public string AsesorNombre { get; set; } = string.Empty;
    public int Orden { get; set; }
    public DateTime? FechaExposicion { get; set; }
    public List<string> Estudiantes { get; set; } = new();
    public int TotalEvaluadores { get; set; }
}
