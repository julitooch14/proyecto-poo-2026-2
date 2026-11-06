using Evaluacion_ArticulosJEAC.Api.Data;
using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Evaluacion_ArticulosJEAC.Api.Services;

namespace Evaluacion_ArticulosJEAC.Tests.Helpers;

public class EscenarioEvaluacion
{
    public AppDbContext Contexto { get; }
    public Docente Asesor { get; }
    public Docente EvaluadorA { get; }
    public Docente EvaluadorB { get; }
    public GrupoExposicion Grupo { get; }
    public EvaluacionServicio Servicio { get; }

    public EscenarioEvaluacion()
    {
        Contexto = FabricaContexto.Crear();

        Asesor = new Docente { Nombre = "Asesor Uno", Correo = "asesor@uco.edu.co", Departamento = "Sistemas" };
        EvaluadorA = new Docente { Nombre = "Evaluador A", Correo = "a@uco.edu.co", Departamento = "Sistemas" };
        EvaluadorB = new Docente { Nombre = "Evaluador B", Correo = "b@uco.edu.co", Departamento = "Sistemas" };

        var articulo = new ArticuloCientifico
        {
            Titulo = "Artículo de prueba",
            Autores = "Autor, A.",
            Revista = "Revista de prueba",
            Anio = 2024,
            Resumen = "Resumen de prueba."
        };

        Grupo = new GrupoExposicion { Nombre = "Grupo de prueba", Articulo = articulo, Asesor = Asesor, Orden = 1 };

        Contexto.Docentes.AddRange(Asesor, EvaluadorA, EvaluadorB);
        Contexto.Grupos.Add(Grupo);
        Contexto.SaveChanges();

        Servicio = new EvaluacionServicio(
            new Repositorio<Evaluacion>(Contexto),
            new Repositorio<GrupoExposicion>(Contexto),
            new Repositorio<Docente>(Contexto));
    }

    public Evaluacion AgregarEvaluacion(Docente evaluador, decimal? nota = null)
    {
        var evaluacion = new Evaluacion { GrupoExposicionId = Grupo.Id, DocenteEvaluadorId = evaluador.Id };

        if (nota.HasValue)
        {
            evaluacion.Calificar(nota.Value, null);
        }

        Contexto.Evaluaciones.Add(evaluacion);
        Contexto.SaveChanges();
        return evaluacion;
    }
}
