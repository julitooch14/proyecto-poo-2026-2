using Modelos.DTO;
using Modelos.Resultados;

namespace Servicios.Interfaces
{
    public interface IBloqueExposicion
    {
        List<BloqueDto> Listar();
        Resultado<BloqueDto> ObtenerMiHorario(int estudianteId);
        Resultado<BloqueDto> Crear(BloqueRequest solicitud);
        Resultado<BloqueDto> Actualizar(int id, BloqueRequest solicitud);
        Resultado<string> Eliminar(int id);
    }
}
