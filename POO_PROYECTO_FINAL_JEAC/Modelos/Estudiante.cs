namespace Modelos
{
    public class Estudiante
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        // Cada estudiante pertenece a un grupo; el artículo de investigación vive en el grupo.
        public int GrupoId { get; set; }
    }
}
