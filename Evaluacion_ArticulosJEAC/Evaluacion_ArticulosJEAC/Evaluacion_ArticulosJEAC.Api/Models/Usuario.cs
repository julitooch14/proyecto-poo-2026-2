namespace Evaluacion_ArticulosJEAC.Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    public int? DocenteId { get; set; }
    public Docente? Docente { get; set; }
}
