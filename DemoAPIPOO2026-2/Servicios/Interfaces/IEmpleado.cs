using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Interfaces
{
    public interface IEmpleado
    {
        public List<Empleado> ObtenerEmpleados();
        public void CrearEmpleado(Empleado empleado);
    }
}
