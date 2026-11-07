namespace jeac.domain.modelos.entidades
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