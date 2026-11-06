namespace Modelos.DTO
{
    public record LoginRequest(string Usuario, string Contrasena);

    public record HorarioRequest(TimeOnly HoraInicio, TimeOnly HoraFin, int? GrupoId);
    public record BloqueRequest(DateOnly Fecha, string Aula, int Evaluador1Id, int Evaluador2Id, List<HorarioRequest> Horarios);

    public record HorarioDto(int Id, TimeOnly HoraInicio, TimeOnly HoraFin, int? GrupoId,
        string? GrupoNombre, string? Articulo, List<string> Estudiantes);
    public record GrupoCatalogoDto(int Id, string Nombre, string Articulo, List<string> Estudiantes);

    public record BloqueDto(int Id, DateOnly Fecha, string Aula,
        int Evaluador1Id, string Evaluador1, int Evaluador2Id, string Evaluador2,
        List<HorarioDto> Horarios);
}
