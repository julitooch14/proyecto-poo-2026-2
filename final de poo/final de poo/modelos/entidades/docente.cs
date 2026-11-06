namespace final_de_poo.modelos.entidades
{
    // Herencia: Hereda de Usuario
    public class Docente : Usuario
    {
        public string Departamento { get; set; } = string.Empty;

        public override string ObtenerRol()
        {
            return "Docente";
        }
    }
}