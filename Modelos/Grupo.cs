namespace Modelos
{
    public class Grupo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string ArticuloDeInvestigacion { get; set; } = string.Empty;
        public List<Estudiante> Estudiantes { get; set; } = new();
    }
}
