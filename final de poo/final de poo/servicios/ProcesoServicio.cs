using final_de_poo.modelos.entidades;

namespace final_de_poo.servicios
{
    public class ProcesoServicio
    {
        public List<Grupo> Grupos { get; set; }
        public List<ArticuloCientifico> Articulos { get; set; }

        public ProcesoServicio()
        {
            Grupos = SemilleroDatos.ObtenerGrupos();
            Articulos = SemilleroDatos.ObtenerArticulos();
        }

        // Asignación Exclusiva por Docente
        public bool AsignarArticuloManualmente(int grupoId, int articuloId, string docenteEmail)
        {
            var grupo = Grupos.FirstOrDefault(g => g.Id == grupoId && g.DocenteEmail == docenteEmail);
            if (grupo == null) return false;

            // Si el docente selecciona "0" o desasigna, liberamos el artículo previo
            if (articuloId == 0)
            {
                if (grupo.ArticuloAsignado != null)
                {
                    grupo.ArticuloAsignado.Asignado = false;
                    grupo.ArticuloAsignado = null;
                }
                return true;
            }

            var nuevoArticulo = Articulos.FirstOrDefault(a => a.Id == articuloId);
            if (nuevoArticulo == null) return false;

            // EXCLUSIVIDAD: Verificar si el artículo ya está asignado a OTRO grupo
            bool yaAsignadoAOtro = Grupos.Any(g => g.Id != grupoId && g.ArticuloAsignado?.Id == articuloId);
            if (yaAsignadoAOtro)
            {
                return false; // Rechaza la asignación porque ya pertenece a otro grupo
            }

            // Liberar el artículo anterior que tenía este grupo (si aplica)
            if (grupo.ArticuloAsignado != null)
            {
                grupo.ArticuloAsignado.Asignado = false;
            }

            // Asignar en exclusiva el nuevo artículo
            grupo.ArticuloAsignado = nuevoArticulo;
            nuevoArticulo.Asignado = true;

            return true;
        }

        public List<Grupo> ConsultarGruposPorDocente(string docenteEmail)
        {
            return Grupos.Where(g => g.DocenteEmail == docenteEmail).ToList();
        }

        public Grupo? ConsultarGrupoPorEstudiante(string estudianteEmail)
        {
            return Grupos.FirstOrDefault(g => g.Estudiantes.Any(e => e.Email == estudianteEmail));
        }

        public List<Grupo> ConsultarTodosLosGrupos()
        {
            return Grupos;
        }

        public List<ArticuloCientifico> ConsultarArticulos()
        {
            return Articulos;
        }
    }
}