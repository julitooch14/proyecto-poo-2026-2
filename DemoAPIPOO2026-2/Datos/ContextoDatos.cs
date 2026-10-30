using Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    public class ContextoDatos
    {

        public static List<Empleado> listaEmpleados = new List<Empleado>()
        {
            new Empleado()
            {
                Id=1,
                Nombre = "Juan",
                Apellido = "Velasquez"
            },
            new Empleado()
            {
                Id=2,
                Nombre = "Salome",
                Apellido = "Cardona"
            },
            new Empleado()
            {
                Id=3,
                Nombre = "Isabela",
                Apellido = "Osorio"
            },
            new Empleado()
            {
                Id=4,
                Nombre = "Steven",
                Apellido = "Gomez"
            }

        };

    }
}
