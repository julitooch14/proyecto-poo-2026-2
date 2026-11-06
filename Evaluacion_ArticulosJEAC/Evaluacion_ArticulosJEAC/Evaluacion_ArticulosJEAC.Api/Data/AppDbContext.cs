using Evaluacion_ArticulosJEAC.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_ArticulosJEAC.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opciones) : base(opciones)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Docente> Docentes => Set<Docente>();
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<ArticuloCientifico> Articulos => Set<ArticuloCientifico>();
    public DbSet<GrupoExposicion> Grupos => Set<GrupoExposicion>();
    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entidad =>
        {
            entidad.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(50);
            entidad.Property(u => u.PasswordHash).IsRequired();
            entidad.HasIndex(u => u.NombreUsuario).IsUnique();
            entidad.HasOne(u => u.Docente)
                .WithOne(d => d.Usuario)
                .HasForeignKey<Usuario>(u => u.DocenteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Docente>(entidad =>
        {
            entidad.Property(d => d.Nombre).IsRequired().HasMaxLength(120);
            entidad.Property(d => d.Correo).IsRequired().HasMaxLength(120);
            entidad.Property(d => d.Departamento).IsRequired().HasMaxLength(120);
            entidad.HasIndex(d => d.Correo).IsUnique();
        });

        modelBuilder.Entity<Estudiante>(entidad =>
        {
            entidad.Property(e => e.Nombre).IsRequired().HasMaxLength(120);
            entidad.Property(e => e.Codigo).IsRequired().HasMaxLength(30);
            entidad.HasIndex(e => e.Codigo).IsUnique();
            entidad.HasOne(e => e.Grupo)
                .WithMany(g => g.Estudiantes)
                .HasForeignKey(e => e.GrupoExposicionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ArticuloCientifico>(entidad =>
        {
            entidad.Property(a => a.Titulo).IsRequired().HasMaxLength(250);
            entidad.Property(a => a.Autores).IsRequired().HasMaxLength(250);
            entidad.Property(a => a.Revista).IsRequired().HasMaxLength(150);
            entidad.Property(a => a.Resumen).IsRequired().HasMaxLength(2000);
        });

        modelBuilder.Entity<GrupoExposicion>(entidad =>
        {
            entidad.Property(g => g.Nombre).IsRequired().HasMaxLength(100);
            entidad.HasOne(g => g.Articulo)
                .WithMany()
                .HasForeignKey(g => g.ArticuloCientificoId)
                .OnDelete(DeleteBehavior.Restrict);
            entidad.HasOne(g => g.Asesor)
                .WithMany(d => d.GruposAsesorados)
                .HasForeignKey(g => g.AsesorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Evaluacion>(entidad =>
        {
            entidad.Property(e => e.Nota).HasPrecision(3, 1);
            entidad.Property(e => e.Observaciones).HasMaxLength(500);
            entidad.HasIndex(e => new { e.GrupoExposicionId, e.DocenteEvaluadorId }).IsUnique();
            entidad.HasOne(e => e.Grupo)
                .WithMany(g => g.Evaluaciones)
                .HasForeignKey(e => e.GrupoExposicionId)
                .OnDelete(DeleteBehavior.Cascade);
            entidad.HasOne(e => e.Evaluador)
                .WithMany(d => d.Evaluaciones)
                .HasForeignKey(e => e.DocenteEvaluadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
