namespace final_de_poo.modelos.entidades
{
    // Herencia: Hereda de Usuario
    public class Administrador : Usuario
    {
        public override string ObtenerRol()
        {
            return "Administrador";
        }
    }
}