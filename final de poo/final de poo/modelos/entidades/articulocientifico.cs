namespace final_de_poo.modelos.entidades
{
    public class ArticuloCientifico
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Resumen { get; set; } = string.Empty;
        public bool Asignado { get; set; } = false;
    }
}