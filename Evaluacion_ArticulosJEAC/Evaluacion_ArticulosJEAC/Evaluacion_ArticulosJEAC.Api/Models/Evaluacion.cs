using Evaluacion_ArticulosJEAC.Api.Exceptions;

namespace Evaluacion_ArticulosJEAC.Api.Models;

public class Evaluacion
{
    public const decimal NotaMinima = 0m;
    public const decimal NotaMaxima = 5m;

    public int Id { get; set; }
    public int GrupoExposicionId { get; set; }
    public GrupoExposicion Grupo { get; set; } = null!;
    public int DocenteEvaluadorId { get; set; }
    public Docente Evaluador { get; set; } = null!;
    public decimal? Nota { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaCalificacion { get; set; }

    public bool EstaCalificada => Nota.HasValue;

    public void Calificar(decimal nota, string? observaciones)
    {
        if (nota < NotaMinima || nota > NotaMaxima)
        {
            throw new ReglaNegocioException($"La nota debe estar entre {NotaMinima:0.0} y {NotaMaxima:0.0}.");
        }

        Nota = decimal.Round(nota, 1, MidpointRounding.AwayFromZero);
        Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim();
        FechaCalificacion = DateTime.UtcNow;
    }
}
