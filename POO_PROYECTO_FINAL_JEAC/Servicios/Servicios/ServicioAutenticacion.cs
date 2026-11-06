using Datos;
using Modelos;
using Servicios.Interfaces;

namespace Servicios.Servicios
{
    public class ServicioAutenticacion : IAutenticacion
    {
        private readonly JeacDbContext _context;

        public ServicioAutenticacion(JeacDbContext context)
        {
            _context = context;
        }

        public Usuario? ValidarCredenciales(string? nombreUsuario, string? contrasena)
        {
            var nombre = (nombreUsuario ?? string.Empty).Trim().ToLower();

            return _context.Usuarios.FirstOrDefault(u =>
                u.NombreUsuario.ToLower() == nombre &&
                u.Contrasena == contrasena);
        }
    }
}
