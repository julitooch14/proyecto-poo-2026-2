using jeac.domain.modelos.entidades;
using System;
using System.Linq;

namespace jeac.services
{
    // Interfaz para el patrón State
    public interface IEtapaProceso
    {
        int NumeroEtapa { get; }
        string Nombre { get; }
        void EjecutarAccion(ProcesoServicio contexto, object datos);
    }

    // Etapa 1: Consolidación de Estudiantes
    public class EtapaConsolidacion : IEtapaProceso
    {
        public int NumeroEtapa => 1;
        public string Nombre => "Consolidación de Estudiantes";

        public void EjecutarAccion(ProcesoServicio contexto, object datos)
        {
            if (datos is Grupo nuevoGrupo)
            {
                if (nuevoGrupo.Estudiantes.Count >= 1 && nuevoGrupo.Estudiantes.Count <= 2)
                {
                    contexto.Grupos.Add(nuevoGrupo);
                }
                else
                {
                    throw new Exception("Un grupo debe tener mínimo 1 y máximo 2 estudiantes.");
                }
            }
        }
    }

    // Etapa 2: Selección y Asignación de Artículos
    public class EtapaAsignacionArticulo : IEtapaProceso
    {
        public int NumeroEtapa => 2;
        public string Nombre => "Selección y Asignación de Artículos";

        public void EjecutarAccion(ProcesoServicio contexto, object datos)
        {
            var (grupoId, articuloId) = ((int, int))datos;

            var grupo = contexto.Grupos.FirstOrDefault(g => g.Id == grupoId);
            var articulo = contexto.Articulos.FirstOrDefault(a => a.Id == articuloId);

            if (grupo == null)
                throw new Exception("El grupo especificado no existe.");

            if (articulo == null)
                throw new Exception("El artículo especificado no existe.");

            if (articulo.Asignado)
                throw new Exception("Este artículo ya fue asignado a otro grupo.");

            grupo.ArticuloAsignado = articulo;
            articulo.Asignado = true;
        }
    }

    // Etapa 3: Consulta de Grupos y Artículos
    public class EtapaConsulta : IEtapaProceso
    {
        public int NumeroEtapa => 3;
        public string Nombre => "Consulta de Grupos y Artículos";

        public void EjecutarAccion(ProcesoServicio contexto, object datos)
        {
            // Operación de lectura de información
        }
    }
}