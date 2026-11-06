using Modelos;

namespace Servicios.Interfaces
{
    public interface IAutenticacion
    {
        // Devuelve el usuario si las credenciales son correctas; null si no lo son.
        Usuario? ValidarCredenciales(string? nombreUsuario, string? contrasena);
    }
}
