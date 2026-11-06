using Evaluacion_ArticulosJEAC.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_ArticulosJEAC.Api.Repositories;

public class Repositorio<T> : IRepositorio<T> where T : class
{
    private readonly AppDbContext _contexto;

    public Repositorio(AppDbContext contexto)
    {
        _contexto = contexto;
    }

    public IQueryable<T> Consultar()
    {
        return _contexto.Set<T>();
    }

    public async Task<T?> ObtenerPorIdAsync(int id)
    {
        return await _contexto.Set<T>().FindAsync(id);
    }

    public async Task AgregarAsync(T entidad)
    {
        await _contexto.Set<T>().AddAsync(entidad);
    }

    public void Eliminar(T entidad)
    {
        _contexto.Set<T>().Remove(entidad);
    }

    public async Task GuardarCambiosAsync()
    {
        await _contexto.SaveChangesAsync();
    }
}
