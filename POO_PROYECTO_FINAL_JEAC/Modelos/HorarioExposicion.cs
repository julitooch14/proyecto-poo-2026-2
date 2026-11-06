namespace Modelos
{
    // Una fila de la tabla: un intervalo de tiempo dentro de un bloque,
    // con un grupo asignado (o libre si GrupoId es null).
    public class HorarioExposicion
    {
        public int Id { get; set; }
        public int BloqueExposicionId { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public int? GrupoId { get; set; }
    }
}
