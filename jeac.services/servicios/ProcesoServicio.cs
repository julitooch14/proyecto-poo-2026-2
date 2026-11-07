using System.Collections.Generic;
using System.Linq;
using jeac.domain.modelos.entidades;
using jeac.data;

namespace jeac.services
{
    public class ProcesoServicio : IEtapaProceso
    {
        // --- Miembros requeridos por IEtapaProceso ---
        public int NumeroEtapa { get; set; } = 1;
        public string Nombre { get; set; } = "Asignación de Artículos";

        // --- Propiedades que redireccionan a la capa de datos (resuelven los errores CS1061) ---
        public List<Grupo> Grupos => SemilleroDatos.Grupos;
        public List<ArticuloCientifico> Articulos => SemilleroDatos.Articulos;

        public void EjecutarAccion(ProcesoServicio servicio, object datos)
        {
            // Implementación requerida por la interfaz (retorno void)
        }

        // --- Métodos de la lógica de negocio ---
        public List<Grupo> ConsultarTodosLosGrupos() => SemilleroDatos.Grupos;

        public List<ArticuloCientifico> ConsultarArticulos() => SemilleroDatos.Articulos;

        public Grupo? ConsultarGrupoPorEstudiante(string email)
        {
            return SemilleroDatos.Grupos.FirstOrDefault(g =>
                g.Estudiantes.Any(e => e.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase)));
        }

        public List<Grupo> ConsultarGruposPorDocente(string docenteEmail)
        {
            return SemilleroDatos.Grupos.Where(g =>
                g.DocenteEmail.Equals(docenteEmail, System.StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public bool AsignarArticuloManualmente(int grupoId, int articuloId, string docenteEmail)
        {
            var grupo = SemilleroDatos.Grupos.FirstOrDefault(g => g.Id == grupoId && g.DocenteEmail == docenteEmail);
            if (grupo == null) return false;

            if (articuloId == 0)
            {
                if (grupo.ArticuloAsignado != null)
                {
                    grupo.ArticuloAsignado.Asignado = false;
                    grupo.ArticuloAsignado = null;
                }
                return true;
            }

            var articulo = SemilleroDatos.Articulos.FirstOrDefault(a => a.Id == articuloId);
            if (articulo == null || articulo.Asignado) return false;

            if (grupo.ArticuloAsignado != null)
            {
                grupo.ArticuloAsignado.Asignado = false;
            }

            articulo.Asignado = true;
            grupo.ArticuloAsignado = articulo;
            return true;
        }
    }
}