namespace final_de_poo.modelos.entidades
{
    public class Estudiante : Usuario
    {
        public string CodigoEstudiantil { get; set; } = string.Empty;

        public override string ObtenerRol()
        {
            return "Estudiante";
        }
    }
}