namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class EstudianteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int? GrupoId { get; set; }
    public string? GrupoNombre { get; set; }
}
