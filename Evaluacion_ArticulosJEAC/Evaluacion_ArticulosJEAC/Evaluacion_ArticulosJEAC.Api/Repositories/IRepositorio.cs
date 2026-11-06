namespace Evaluacion_ArticulosJEAC.Api.Repositories;

public interface IRepositorio<T> where T : class
{
    IQueryable<T> Consultar();
    Task<T?> ObtenerPorIdAsync(int id);
    Task AgregarAsync(T entidad);
    void Eliminar(T entidad);
    Task GuardarCambiosAsync();
}
