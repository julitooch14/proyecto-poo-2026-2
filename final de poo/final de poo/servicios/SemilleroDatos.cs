using final_de_poo.modelos.entidades;

namespace final_de_poo.servicios
{
    public static class SemilleroDatos
    {
        public static List<Grupo> ObtenerGrupos()
        {
            return new List<Grupo>
            {
                // GRUPOS A CARGO DE JORGE MARIO GARZÓN (1 al 5)
                new Grupo
                {
                    Id = 1,
                    NombreGrupo = "Grupo #1 ",
                    DocenteEmail = "jorge.mario@uco.edu.co",
                    DocenteNombre = "Jorge Mario Garzón",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 1, Nombre = "Tulio Ospina", Email = "tulio.ospina@udo.edu.co" },
                        new Estudiante { Id = 2, Nombre = "Mariana Gómez", Email = "mariana.gomez@udo.edu.co" }
                    }
                },
                new Grupo
                {
                    Id = 2,
                    NombreGrupo = "Grupo #2 ",
                    DocenteEmail = "jorge.mario@uco.edu.co",
                    DocenteNombre = "Jorge Mario Garzón",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 3, Nombre = "Santiago Restrepo", Email = "santiago.restrepo@udo.edu.co" },
                        new Estudiante { Id = 4, Nombre = "Valentina Jaramillo", Email = "valentina.jaramillo@udo.edu.co" }
                    }
                },
                new Grupo
                {
                    Id = 3,
                    NombreGrupo = "Grupo #3 ",
                    DocenteEmail = "jorge.mario@uco.edu.co",
                    DocenteNombre = "Jorge Mario Garzón",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 5, Nombre = "Mateo Castrillón", Email = "mateo.castrillon@udo.edu.co" },
                        new Estudiante { Id = 6, Nombre = "Isabella Lopera", Email = "isabella.lopera@udo.edu.co" }
                    }
                },
                new Grupo
                {
                    Id = 4,
                    NombreGrupo = "Grupo #4 ",
                    DocenteEmail = "jorge.mario@uco.edu.co",
                    DocenteNombre = "Jorge Mario Garzón",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 7, Nombre = "Samuel Morales", Email = "samuel.morales@udo.edu.co" },
                        new Estudiante { Id = 8, Nombre = "Camila Bermúdez", Email = "camila.bermudez@udo.edu.co" }
                    }
                },
                new Grupo
                {
                    Id = 5,
                    NombreGrupo = "Grupo #5 ",
                    DocenteEmail = "jorge.mario@uco.edu.co",
                    DocenteNombre = "Jorge Mario Garzón",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 9, Nombre = "Alejandro Zuluaga", Email = "alejandro.zuluaga@udo.edu.co" },
                        new Estudiante { Id = 10, Nombre = "Daniela Henao", Email = "daniela.henao@udo.edu.co" }
                    }
                },

                // GRUPOS A CARGO DE RICARDO COGOLLO (6 al 10)
                new Grupo
                {
                    Id = 6,
                    NombreGrupo = "Grupo #6",
                    DocenteEmail = "ricardo.cogollo@uco.edu.co",
                    DocenteNombre = "Ricardo Cogollo",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 11, Nombre = "Nicolás Ramírez", Email = "nicolas.ramirez@udo.edu.co" },
                        new Estudiante { Id = 12, Nombre = "Sofía Montoya", Email = "sofia.montoya@udo.edu.co" }
                    }
                },
                new Grupo
                {
                    Id = 7,
                    NombreGrupo = "Grupo #7 ",
                    DocenteEmail = "ricardo.cogollo@uco.edu.co",
                    DocenteNombre = "Ricardo Cogollo",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 13, Nombre = "Tomas Cardona", Email = "tomas.cardona@udo.edu.co" },
                        new Estudiante { Id = 14, Nombre = "Luciana Giraldo", Email = "luciana.giraldo@udo.edu.co" }
                    }
                },
                new Grupo
                {
                    Id = 8,
                    NombreGrupo = "Grupo #8 ",
                    DocenteEmail = "ricardo.cogollo@uco.edu.co",
                    DocenteNombre = "Ricardo Cogollo",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 15, Nombre = "Emmanuel Vargas", Email = "emmanuel.vargas@udo.edu.co" },
                        new Estudiante { Id = 16, Nombre = "Salomé Arango", Email = "salome.arango@udo.edu.co" }
                    }
                },
                new Grupo
                {
                    Id = 9,
                    NombreGrupo = "Grupo #9 ",
                    DocenteEmail = "ricardo.cogollo@uco.edu.co",
                    DocenteNombre = "Ricardo Cogollo",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 17, Nombre = "Jerónimo Quintero", Email = "jeronimo.quintero@udo.edu.co" },
                        new Estudiante { Id = 18, Nombre = "Valery Osorio", Email = "valery.osorio@udo.edu.co" }
                    }
                },
                new Grupo
                {
                    Id = 10,
                    NombreGrupo = "Grupo #10 ",
                    DocenteEmail = "ricardo.cogollo@uco.edu.co",
                    DocenteNombre = "Ricardo Cogollo",
                    Estudiantes = new List<Estudiante>
                    {
                        new Estudiante { Id = 19, Nombre = "Sebastián Marín", Email = "sebastian.marin@udo.edu.co" },
                        new Estudiante { Id = 20, Nombre = "Gabriela Cano", Email = "gabriela.cano@udo.edu.co" }
                    }
                }
            };
        }

        public static List<ArticuloCientifico> ObtenerArticulos()
        {
            return new List<ArticuloCientifico>
            {
                new ArticuloCientifico
                {
                    Id = 1,
                    Titulo = "Implementación de JWT en Arquitecturas Limpias con .NET 8",
                    Resumen = "Un estudio sobre la seguridad distribuida mediante tokens bearer sin estado en microservicios modernos.",
                    Asignado = false
                },
                new ArticuloCientifico
                {
                    Id = 2,
                    Titulo = "Patrones SOLID Aplicados a la Programación Orientada a Objetos",
                    Resumen = "Análisis de desacoplamiento de componentes y mantenibilidad de código en proyectos a gran escala.",
                    Asignado = false
                },
                new ArticuloCientifico
                {
                    Id = 3,
                    Titulo = "Optimización de Consultas con Entity Framework Core 8",
                    Resumen = "Evaluación de rendimiento en mapeo objeto-relacional y estrategias de seguimiento de estado.",
                    Asignado = false
                },
                new ArticuloCientifico
                {
                    Id = 4,
                    Titulo = "Principios de Diseño de APIs RESTful Escalables",
                    Resumen = "Estándares de versionamiento, manejo de errores unificado y códigos de estado HTTP en servicios web.",
                    Asignado = false
                },
                new ArticuloCientifico
                {
                    Id = 5,
                    Titulo = "Seguridad Web: Mitigación de Vulnerabilidades OWASP Top 10",
                    Resumen = "Estrategias de defensa en código fuente contra inyecciones SQL, XSS y falsificación de peticiones.",
                    Asignado = false
                },
                new ArticuloCientifico
                {
                    Id = 6,
                    Titulo = "Uso de Event-Driven Architecture en Sistemas Distribuidos",
                    Resumen = "Estudio comparativo de brokers de mensajería y patrones de publicación/suscripción en C#.",
                    Asignado = false
                },
                new ArticuloCientifico
                {
                    Id = 7,
                    Titulo = "Inyección de Dependencias y Ciclos de Vida Scoped, Transient y Singleton",
                    Resumen = "Impacto del manejo de memoria e hilos concurrentes en servidores de aplicaciones web ASP.NET Core.",
                    Asignado = false
                },
                new ArticuloCientifico
                {
                    Id = 8,
                    Titulo = "Pruebas Unitarias e Integración con xUnit y Moq",
                    Resumen = "Metodologías de desarrollo guiado por pruebas (TDD) para garantizar la calidad en software bancario.",
                    Asignado = false
                },
                new ArticuloCientifico
                {
                    Id = 9,
                    Titulo = "Despliegue Continuo (CI/CD) de Web APIs mediante Docker y Kubernetes",
                    Resumen = "Automatización de contenedores e infraestructura como código para entornos de producción resilientes.",
                    Asignado = false
                },
                new ArticuloCientifico
                {
                    Id = 10,
                    Titulo = "Refactorización de Código Legado utilizando Patrones GoF",
                    Resumen = "Casos prácticos de transformación de sistemas monolíticos hacia arquitecturas flexibles y modulares.",
                    Asignado = false
                }
            };
        }
    }
}