namespace Modelos
{
    // Un bloque = un salón + una fecha + 2 docentes evaluadores + varios horarios.
    public class BloqueExposicion
    {
        public int Id { get; set; }
        public DateOnly Fecha { get; set; }
        public string Aula { get; set; } = string.Empty;
        public int Evaluador1Id { get; set; }
        public int Evaluador2Id { get; set; }
        public List<HorarioExposicion> Horarios { get; set; } = new();
    }
}
