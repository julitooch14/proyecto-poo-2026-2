
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Modelos;
using Modelos.DTOs;
using Servicios.Interfaces;
using System.Collections.Generic;

namespace DemoAPIPOO2026_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly IEmpleado _empleado;
        public EmpleadoController(IEmpleado empleado) { 
            this._empleado = empleado;
        }

        
        //[HttpGet("{id}")]
        [HttpGet("GetEmpleadoPorId")]
        public IActionResult GetEmpleadoPorId(int id)
        {
            var empleado = this._empleado.ObtenerEmpleados().Where(item=>item.Id == id).FirstOrDefault();
            
            return Ok(empleado);
        }

        [HttpGet("GetEmpleadoPorNombre")]
        public IActionResult GetEmpleadoPorNombre(string nombre)
        {
            var empleados = this._empleado.ObtenerEmpleados().Where(item => item.Nombre.ToLower().Contains(nombre.ToLower())).ToList();

            return Ok(empleados);
        }



        [HttpGet("ObtenerTodosLosEmpleados")]
        public IActionResult GetEmpleados() { 
            return Ok(this._empleado.ObtenerEmpleados());
        }
        
        
        
        [HttpPost]
        public IActionResult AddEmpleado(EmpleadoDTO empleadoDto)
        {

            int nuevoId = this._empleado.ObtenerEmpleados().Count + 1;

            var empleadoBD = new Empleado();
            empleadoBD.Id = nuevoId;
            empleadoBD.Nombre = empleadoDto.Nombre;
            empleadoBD.Apellido = empleadoDto.Apellido;

            this._empleado.CrearEmpleado(empleadoBD);
            return StatusCode(StatusCodes.Status201Created);
        }
        

    }
}
