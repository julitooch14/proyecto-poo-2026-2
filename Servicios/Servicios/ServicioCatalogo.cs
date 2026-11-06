using Datos;
using Microsoft.EntityFrameworkCore;
using Modelos;
using Modelos.DTO;
using Servicios.Interfaces;

namespace Servicios.Servicios
{
    // Listas que el administrador necesita para armar un bloque:
    // qué grupos existen y qué docentes puede asignar.
    public class ServicioCatalogo : ICatalogo
    {
        private readonly JeacDbContext _context;

        public ServicioCatalogo(JeacDbContext context)
        {
            _context = context;
        }

        public List<GrupoCatalogoDto> ObtenerGrupos()
        {
            return _context.Grupos
                .Include(g => g.Estudiantes)
                .OrderBy(g => g.Id)
                .ToList()
                .Select(g => new GrupoCatalogoDto(
                    g.Id,
                    g.Nombre,
                    g.ArticuloDeInvestigacion,
                    g.Estudiantes.Select(e => e.Nombre).ToList()))
                .ToList();
        }

        public List<Evaluador> ObtenerEvaluadores()
        {
            return _context.Evaluadores
                .Where(e => e.Disponible)
                .OrderBy(e => e.Nombre)
                .ToList();
        }
    }
}
