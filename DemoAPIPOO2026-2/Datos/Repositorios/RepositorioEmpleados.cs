using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.Repositorios
{
    public class RepositorioEmpleados
    {
        private readonly AppDbContext _context;

        public RepositorioEmpleados(AppDbContext context)
        {
            _context = context;
        }

        public List<Empleado> ObtenerTodos() => _context.Empleados.ToList();

        public void CrearEmpleado(Empleado empleado) { 
            _context.Empleados.Add(empleado);
        }
    }
}
