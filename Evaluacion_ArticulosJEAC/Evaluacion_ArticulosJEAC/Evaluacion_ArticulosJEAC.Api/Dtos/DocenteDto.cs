namespace Evaluacion_ArticulosJEAC.Api.Dtos;

public class DocenteDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string? NombreUsuario { get; set; }
}
