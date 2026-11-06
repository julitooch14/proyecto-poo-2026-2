namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class RespuestaLoginDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expira { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? DocenteId { get; set; }
    public string? NombreDocente { get; set; }
}
