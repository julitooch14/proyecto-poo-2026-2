namespace Modelos
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        // "Estudiante", "Docente" o "Administrador".
        public string Rol { get; set; } = string.Empty;
        // Solo para usuarios con rol Estudiante: a qué estudiante corresponde la cuenta.
        public int? EstudianteId { get; set; }
    }
}
