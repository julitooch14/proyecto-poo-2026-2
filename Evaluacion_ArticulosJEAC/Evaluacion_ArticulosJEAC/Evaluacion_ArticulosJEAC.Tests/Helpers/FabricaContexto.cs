using Evaluacion_ArticulosJEAC.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_ArticulosJEAC.Tests.Helpers;

public static class FabricaContexto
{
    public static AppDbContext Crear()
    {
        var opciones = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(opciones);
    }
}
