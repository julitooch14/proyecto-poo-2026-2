namespace final_de_poo.modelos.entidades
{
    public class Grupo
    {
        public int Id { get; set; }
        public string NombreGrupo { get; set; } = string.Empty;
        public string DocenteEmail { get; set; } = string.Empty;
        public string DocenteNombre { get; set; } = string.Empty;
        public List<Estudiante> Estudiantes { get; set; } = new List<Estudiante>();
        public ArticuloCientifico? ArticuloAsignado { get; set; }
    }
}