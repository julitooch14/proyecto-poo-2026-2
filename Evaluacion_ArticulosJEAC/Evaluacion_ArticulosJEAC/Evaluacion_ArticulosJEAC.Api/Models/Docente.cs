namespace Evaluacion_ArticulosJEAC.Api.Models;

public class Docente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public Usuario? Usuario { get; set; }
    public ICollection<GrupoExposicion> GruposAsesorados { get; set; } = new List<GrupoExposicion>();
    public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
}
