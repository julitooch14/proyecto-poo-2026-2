using Evaluacion_ArticulosJEAC.Api.Dtos;
using Evaluacion_ArticulosJEAC.Api.Exceptions;
using Evaluacion_ArticulosJEAC.Api.Mappers;
using Evaluacion_ArticulosJEAC.Api.Models;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Evaluacion_ArticulosJEAC.Api.Services;

public class ArticuloServicio : IArticuloServicio
{
    private readonly IRepositorio<ArticuloCientifico> _articulos;
    private readonly IRepositorio<GrupoExposicion> _grupos;

    public ArticuloServicio(IRepositorio<ArticuloCientifico> articulos, IRepositorio<GrupoExposicion> grupos)
    {
        _articulos = articulos;
        _grupos = grupos;
    }

    public async Task<List<ArticuloDto>> ListarAsync()
    {
        var articulos = await _articulos.Consultar()
            .AsNoTracking()
            .OrderBy(a => a.Titulo)
            .ToListAsync();

        return articulos.Select(a => Mapeador.AArticuloDto(a)).ToList();
    }

    public async Task<ArticuloDto> CrearAsync(CrearArticuloDto dto)
    {
        var articulo = new ArticuloCientifico
        {
            Titulo = dto.Titulo.Trim(),
            Autores = dto.Autores.Trim(),
            Revista = dto.Revista.Trim(),
            Anio = dto.Anio,
            Resumen = dto.Resumen.Trim()
        };

        await _articulos.AgregarAsync(articulo);
        await _articulos.GuardarCambiosAsync();

        return Mapeador.AArticuloDto(articulo);
    }

    public async Task EliminarAsync(int id)
    {
        var articulo = await _articulos.ObtenerPorIdAsync(id)
            ?? throw new RecursoNoEncontradoException("El artículo no existe.");

        if (await _grupos.Consultar().AnyAsync(g => g.ArticuloCientificoId == id))
        {
            throw new ReglaNegocioException("No se puede eliminar un artículo asignado a un grupo.");
        }

        _articulos.Eliminar(articulo);
        await _articulos.GuardarCambiosAsync();
    }
}
