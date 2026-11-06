namespace Evaluacion_ArticulosJEAC.Api.Models;

public class GrupoExposicion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int ArticuloCientificoId { get; set; }
    public ArticuloCientifico Articulo { get; set; } = null!;
    public int AsesorId { get; set; }
    public Docente Asesor { get; set; } = null!;
    public int Orden { get; set; }
    public DateTime? FechaExposicion { get; set; }
    public ICollection<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
    public ICollection<Evaluacion> Evaluaciones { get; set; } = new List<Evaluacion>();
}
