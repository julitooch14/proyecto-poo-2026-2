namespace Evaluacion_ArticulosJEAC.Api.Models;

public class ArticuloCientifico
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autores { get; set; } = string.Empty;
    public string Revista { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Resumen { get; set; } = string.Empty;
}
