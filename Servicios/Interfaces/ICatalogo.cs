using Modelos;
using Modelos.DTO;

namespace Servicios.Interfaces
{
    public interface ICatalogo
    {
        List<GrupoCatalogoDto> ObtenerGrupos();
        List<Evaluador> ObtenerEvaluadores();
    }
}
