namespace final_de_poo.modelos.entidades
{
    // Abstracción: Clase base de usuario
    public abstract class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Polimorfismo: Cada rol devolverá su propio nombre de rol
        public abstract string ObtenerRol();
    }
}