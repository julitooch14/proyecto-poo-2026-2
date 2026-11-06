using Microsoft.EntityFrameworkCore;
using Modelos;

namespace Datos
{
    public class JeacDbContext : DbContext
    {
        public JeacDbContext(DbContextOptions<JeacDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Evaluador> Evaluadores => Set<Evaluador>();
        public DbSet<Grupo> Grupos => Set<Grupo>();
        public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
        public DbSet<BloqueExposicion> Bloques => Set<BloqueExposicion>();
        public DbSet<HorarioExposicion> Horarios => Set<HorarioExposicion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Datos iniciales para las pruebas. En este proyecto los docentes
            // se representan mediante la entidad Evaluador.
            modelBuilder.Entity<Evaluador>().HasData(
                new Evaluador { Id = 1, Nombre = "Docente 1", Disponible = true },
                new Evaluador { Id = 2, Nombre = "Docente 2", Disponible = true },
                new Evaluador { Id = 3, Nombre = "Docente 3", Disponible = true },
                new Evaluador { Id = 4, Nombre = "Docente 4", Disponible = true });

            modelBuilder.Entity<Grupo>().HasData(
                new Grupo { Id = 1, Nombre = "Grupo 1", ArticuloDeInvestigacion = "Inteligencia artificial aplicada a la educación" },
                new Grupo { Id = 2, Nombre = "Grupo 2", ArticuloDeInvestigacion = "Sistemas distribuidos para universidades" },
                new Grupo { Id = 3, Nombre = "Grupo 3", ArticuloDeInvestigacion = "Aplicaciones móviles educativas" });

            modelBuilder.Entity<Estudiante>().HasData(
                new Estudiante { Id = 1, Nombre = "Estudiante 1", Correo = "estudiante1@jeac.com", GrupoId = 1 },
                new Estudiante { Id = 2, Nombre = "Estudiante 2", Correo = "estudiante2@jeac.com", GrupoId = 1 },
                new Estudiante { Id = 3, Nombre = "Estudiante 3", Correo = "estudiante3@jeac.com", GrupoId = 2 },
                new Estudiante { Id = 4, Nombre = "Estudiante 4", Correo = "estudiante4@jeac.com", GrupoId = 2 },
                new Estudiante { Id = 5, Nombre = "Estudiante 5", Correo = "estudiante5@jeac.com", GrupoId = 3 },
                new Estudiante { Id = 6, Nombre = "Estudiante 6", Correo = "estudiante6@jeac.com", GrupoId = 3 });

            // Cuentas de prueba (contraseña 1234 para todas).
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { Id = 1, NombreUsuario = "administrador", Contrasena = "1234", Nombre = "Administrador", Rol = "Administrador" },
                new Usuario { Id = 2, NombreUsuario = "docente1", Contrasena = "1234", Nombre = "Docente 1", Rol = "Docente" },
                new Usuario { Id = 3, NombreUsuario = "docente2", Contrasena = "1234", Nombre = "Docente 2", Rol = "Docente" },
                new Usuario { Id = 4, NombreUsuario = "estudiante1", Contrasena = "1234", Nombre = "Estudiante 1", Rol = "Estudiante", EstudianteId = 1 },
                new Usuario { Id = 5, NombreUsuario = "estudiante2", Contrasena = "1234", Nombre = "Estudiante 2", Rol = "Estudiante", EstudianteId = 2 },
                new Usuario { Id = 6, NombreUsuario = "estudiante3", Contrasena = "1234", Nombre = "Estudiante 3", Rol = "Estudiante", EstudianteId = 3 },
                new Usuario { Id = 7, NombreUsuario = "estudiante4", Contrasena = "1234", Nombre = "Estudiante 4", Rol = "Estudiante", EstudianteId = 4 },
                new Usuario { Id = 8, NombreUsuario = "estudiante5", Contrasena = "1234", Nombre = "Estudiante 5", Rol = "Estudiante", EstudianteId = 5 },
                new Usuario { Id = 9, NombreUsuario = "estudiante6", Contrasena = "1234", Nombre = "Estudiante 6", Rol = "Estudiante", EstudianteId = 6 });
        }
    }
}
