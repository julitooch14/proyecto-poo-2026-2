using Datos;
using Datos.Repositorios;
using Modelos;
using Servicios.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Servicios
{
    public class ServicioEmpleado:IEmpleado
    {
        private readonly RepositorioEmpleados _repositorio;

        public ServicioEmpleado(AppDbContext context)
        {
            _repositorio = new RepositorioEmpleados(context);
        }
        public List<Empleado> ObtenerEmpleados() {
            return ContextoDatos.listaEmpleados;
            //return this._repositorio.ObtenerTodos();
        }

        public void CrearEmpleado(Empleado empleado)
        {
            ContextoDatos.listaEmpleados.Add(empleado);
            //this._repositorio.CrearEmpleado(empleado);
        }

    }
}
