using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Security;

namespace Evaluacion_ArticulosJEAC.Api.Data;

public static class DatosIniciales
{
    public static void Sembrar(AppDbContext contexto, IHasheadorContrasenas hasheador)
    {
        if (contexto.Docentes.Any() || contexto.Usuarios.Any())
        {
            return;
        }

        var carlos = new Docente { Nombre = "Carlos Andrés Mejía", Correo = "carlos.mejia@uco.edu.co", Departamento = "Ingeniería de Sistemas" };
        var maria = new Docente { Nombre = "María Fernanda Ospina", Correo = "maria.ospina@uco.edu.co", Departamento = "Ingeniería de Sistemas" };
        var luis = new Docente { Nombre = "Luis Alberto Zapata", Correo = "luis.zapata@uco.edu.co", Departamento = "Ingeniería Ambiental" };
        var diana = new Docente { Nombre = "Diana Patricia Giraldo", Correo = "diana.giraldo@uco.edu.co", Departamento = "Ingeniería de Sistemas" };
        var andres = new Docente { Nombre = "Andrés Felipe Restrepo", Correo = "andres.restrepo@uco.edu.co", Departamento = "Ingeniería Ambiental" };

        var a1 = new ArticuloCientifico
        {
            Titulo = "Aprendizaje profundo para la detección temprana de plagas en cultivos de papa",
            Autores = "Gómez, R.; Pineda, L.; Arango, S.",
            Revista = "Revista Colombiana de Computación",
            Anio = 2023,
            Resumen = "Se entrena una red neuronal convolucional con imágenes de hojas para identificar plagas comunes en cultivos de papa, alcanzando una exactitud superior al 92 % en condiciones de campo."
        };
        var a2 = new ArticuloCientifico
        {
            Titulo = "Arquitecturas de microservicios en sistemas de información universitarios",
            Autores = "Henao, J.; Ruiz, M.",
            Revista = "Ingeniería y Competitividad",
            Anio = 2022,
            Resumen = "Se comparan arquitecturas monolíticas y de microservicios aplicadas a sistemas académicos, evaluando escalabilidad, mantenibilidad y costos de despliegue."
        };
        var a3 = new ArticuloCientifico
        {
            Titulo = "Internet de las cosas para el monitoreo del recurso hídrico en cuencas andinas",
            Autores = "Cardona, P.; Montoya, V.; Salazar, D.",
            Revista = "Ciencia e Ingeniería Ambiental",
            Anio = 2024,
            Resumen = "Se despliega una red de sensores de bajo costo para medir caudal y calidad del agua, transmitiendo los datos a una plataforma de análisis en tiempo real."
        };
        var a4 = new ArticuloCientifico
        {
            Titulo = "Pruebas unitarias y calidad del software en proyectos académicos",
            Autores = "Vélez, A.; Torres, C.",
            Revista = "Educación en Ingeniería",
            Anio = 2021,
            Resumen = "Se analiza el efecto de incorporar pruebas unitarias desde el inicio del curso sobre la cantidad de defectos y la calidad del diseño en proyectos estudiantiles."
        };

        var g1 = new GrupoExposicion { Nombre = "Grupo 1", Articulo = a1, Asesor = diana, Orden = 1, FechaExposicion = new DateTime(2026, 11, 20, 8, 0, 0) };
        var g2 = new GrupoExposicion { Nombre = "Grupo 2", Articulo = a2, Asesor = andres, Orden = 2, FechaExposicion = new DateTime(2026, 11, 20, 9, 0, 0) };
        var g3 = new GrupoExposicion { Nombre = "Grupo 3", Articulo = a3, Asesor = diana, Orden = 3, FechaExposicion = new DateTime(2026, 11, 20, 10, 0, 0) };
        var g4 = new GrupoExposicion { Nombre = "Grupo 4", Articulo = a4, Asesor = carlos, Orden = 4, FechaExposicion = new DateTime(2026, 11, 20, 11, 0, 0) };

        var estudiantes = new List<Estudiante>
        {
            new() { Nombre = "Valentina Castaño Ríos", Codigo = "EST-001", Grupo = g1 },
            new() { Nombre = "Santiago Muñoz Álvarez", Codigo = "EST-002", Grupo = g1 },
            new() { Nombre = "Camila Duque Herrera", Codigo = "EST-003", Grupo = g2 },
            new() { Nombre = "Mateo Agudelo Posada", Codigo = "EST-004", Grupo = g2 },
            new() { Nombre = "Isabela Franco Londoño", Codigo = "EST-005", Grupo = g3 },
            new() { Nombre = "Julián Cardona Builes", Codigo = "EST-006", Grupo = g3 },
            new() { Nombre = "Sara Orozco Marín", Codigo = "EST-007", Grupo = g4 },
            new() { Nombre = "Daniel Ceballos Soto", Codigo = "EST-008", Grupo = g4 },
            new() { Nombre = "Laura Quintero Zuluaga", Codigo = "EST-009" }
        };

        var evaluacionCalificada = new Evaluacion { Grupo = g1, Evaluador = maria };
        evaluacionCalificada.Calificar(4.5m, "Buena claridad metodológica y presentación de resultados.");

        var evaluaciones = new List<Evaluacion>
        {
            new() { Grupo = g1, Evaluador = carlos },
            evaluacionCalificada,
            new() { Grupo = g2, Evaluador = carlos },
            new() { Grupo = g2, Evaluador = luis },
            new() { Grupo = g3, Evaluador = maria },
            new() { Grupo = g3, Evaluador = luis },
            new() { Grupo = g4, Evaluador = maria },
            new() { Grupo = g4, Evaluador = luis }
        };

        var usuarios = new List<Usuario>
        {
            new() { NombreUsuario = "admin", PasswordHash = hasheador.Hashear("admin123"), Rol = RolUsuario.Admin },
            new() { NombreUsuario = "user", PasswordHash = hasheador.Hashear("user123"), Rol = RolUsuario.Docente, Docente = carlos },
            new() { NombreUsuario = "maria", PasswordHash = hasheador.Hashear("maria123"), Rol = RolUsuario.Docente, Docente = maria },
            new() { NombreUsuario = "luis", PasswordHash = hasheador.Hashear("luis123"), Rol = RolUsuario.Docente, Docente = luis }
        };

        contexto.Docentes.AddRange(carlos, maria, luis, diana, andres);
        contexto.Articulos.AddRange(a1, a2, a3, a4);
        contexto.Grupos.AddRange(g1, g2, g3, g4);
        contexto.Estudiantes.AddRange(estudiantes);
        contexto.Evaluaciones.AddRange(evaluaciones);
        contexto.Usuarios.AddRange(usuarios);
        contexto.SaveChanges();
    }
}
