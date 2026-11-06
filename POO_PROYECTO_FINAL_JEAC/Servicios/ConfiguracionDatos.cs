using Datos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Servicios
{
    // Único lugar que conoce cómo se monta la capa de Datos.
    // Así el proyecto API solo necesita conocer Servicios y Modelos, nunca Datos.
    public static class ConfiguracionDatos
    {
        public static IServiceCollection AgregarBaseDeDatos(this IServiceCollection services)
        {
            services.AddDbContext<JeacDbContext>(options =>
                options.UseInMemoryDatabase("JeacPruebas"));
            return services;
        }

        // Crea la base de datos en memoria y carga los datos de prueba definidos
        // en OnModelCreating.
        public static void InicializarBaseDeDatos(this IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<JeacDbContext>();
            context.Database.EnsureCreated();
        }
    }
}
