namespace Evaluacion_ArticulosJEAC.Api.Models;

public class Estudiante
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public int? GrupoExposicionId { get; set; }
    public GrupoExposicion? Grupo { get; set; }
}
