using Datos;
using Microsoft.EntityFrameworkCore;
using Modelos;
using Modelos.DTO;
using Modelos.Resultados;
using Servicios.Interfaces;

namespace Servicios.Servicios
{
    public class ServicioBloqueExposicion : IBloqueExposicion
    {
        private readonly JeacDbContext _context;

        public ServicioBloqueExposicion(JeacDbContext context)
        {
            _context = context;
        }

        public List<BloqueDto> Listar()
        {
            var bloques = _context.Bloques
                .Include(b => b.Horarios)
                .OrderBy(b => b.Fecha)
                .ThenBy(b => b.Aula)
                .ToList();

            return Convertir(bloques);
        }

        public Resultado<BloqueDto> ObtenerMiHorario(int estudianteId)
        {
            var estudiante = _context.Estudiantes.Find(estudianteId);
            if (estudiante is null)
                return Resultado<BloqueDto>.Falla(TipoResultado.NoAutorizado, "El estudiante del token no existe.");

            var bloque = _context.Bloques
                .Include(b => b.Horarios)
                .FirstOrDefault(b => b.Horarios.Any(h => h.GrupoId == estudiante.GrupoId));

            if (bloque is null)
                return Resultado<BloqueDto>.Falla(TipoResultado.NoEncontrado, "Tu grupo aún no tiene una exposición asignada.");

            return Resultado<BloqueDto>.Exito(Convertir(new[] { bloque }, estudiante.GrupoId)[0]);
        }

        // Guardar un bloque completo (salón, docentes y todos sus horarios) en una sola operación.
        public Resultado<BloqueDto> Crear(BloqueRequest solicitud)
        {
            var problema = Validar(solicitud, 0);
            if (problema is not null)
                return Resultado<BloqueDto>.Falla(problema.Value.Tipo, problema.Value.Mensaje);

            var bloque = new BloqueExposicion();
            Copiar(bloque, solicitud);
            _context.Bloques.Add(bloque);
            _context.SaveChanges();

            return Resultado<BloqueDto>.Exito(Convertir(new[] { bloque })[0]);
        }

        public Resultado<BloqueDto> Actualizar(int id, BloqueRequest solicitud)
        {
            var bloque = _context.Bloques.Include(b => b.Horarios).FirstOrDefault(b => b.Id == id);
            if (bloque is null)
                return Resultado<BloqueDto>.Falla(TipoResultado.NoEncontrado, "No se encontró el bloque indicado.");

            var problema = Validar(solicitud, id);
            if (problema is not null)
                return Resultado<BloqueDto>.Falla(problema.Value.Tipo, problema.Value.Mensaje);

            _context.Horarios.RemoveRange(bloque.Horarios);
            Copiar(bloque, solicitud);
            _context.SaveChanges();

            return Resultado<BloqueDto>.Exito(Convertir(new[] { bloque })[0]);
        }

        public Resultado<string> Eliminar(int id)
        {
            var bloque = _context.Bloques.Include(b => b.Horarios).FirstOrDefault(b => b.Id == id);
            if (bloque is null)
                return Resultado<string>.Falla(TipoResultado.NoEncontrado, "No se encontró el bloque indicado.");

            _context.Bloques.Remove(bloque);
            _context.SaveChanges();

            return Resultado<string>.Exito("Bloque eliminado correctamente.");
        }

        private static void Copiar(BloqueExposicion bloque, BloqueRequest solicitud)
        {
            bloque.Fecha = solicitud.Fecha;
            bloque.Aula = solicitud.Aula.Trim();
            bloque.Evaluador1Id = solicitud.Evaluador1Id;
            bloque.Evaluador2Id = solicitud.Evaluador2Id;
            bloque.Horarios = solicitud.Horarios
                .Select(h => new HorarioExposicion
                {
                    HoraInicio = h.HoraInicio,
                    HoraFin = h.HoraFin,
                    GrupoId = h.GrupoId
                })
                .ToList();
        }

        // Reglas que el servidor garantiza sin importar lo que haga el front end.
        // idActual es el bloque que se está editando (0 si es nuevo) para no chocar consigo mismo.
        // Devuelve null si todo está bien, o el tipo de problema y su mensaje.
        private (TipoResultado Tipo, string Mensaje)? Validar(BloqueRequest s, int idActual)
        {
            if (string.IsNullOrWhiteSpace(s.Aula))
                return (TipoResultado.SolicitudInvalida, "Debes indicar el salón.");

            if (s.Horarios is null || s.Horarios.Count == 0)
                return (TipoResultado.SolicitudInvalida, "El bloque debe tener al menos un horario.");

            if (s.Evaluador1Id == s.Evaluador2Id)
                return (TipoResultado.SolicitudInvalida, "Debes elegir dos docentes distintos.");

            var docentesValidos = _context.Evaluadores
                .Count(e => e.Disponible && (e.Id == s.Evaluador1Id || e.Id == s.Evaluador2Id));
            if (docentesValidos != 2)
                return (TipoResultado.SolicitudInvalida, "Los dos docentes deben existir y estar disponibles.");

            // Horarios válidos y sin solaparse entre sí.
            var ordenados = s.Horarios.OrderBy(h => h.HoraInicio).ToList();
            for (var i = 0; i < ordenados.Count; i++)
            {
                if (ordenados[i].HoraInicio >= ordenados[i].HoraFin)
                    return (TipoResultado.SolicitudInvalida, "Cada horario debe terminar después de empezar.");
                if (i > 0 && ordenados[i].HoraInicio < ordenados[i - 1].HoraFin)
                    return (TipoResultado.SolicitudInvalida, "Los horarios de un bloque no pueden solaparse.");
            }

            // Grupos: existentes, sin repetirse y sin estar ya en otro bloque.
            var grupos = s.Horarios.Where(h => h.GrupoId.HasValue).Select(h => h.GrupoId!.Value).ToList();
            if (grupos.Distinct().Count() != grupos.Count)
                return (TipoResultado.SolicitudInvalida, "Un grupo no puede aparecer dos veces en el mismo bloque.");

            if (grupos.Any(g => !_context.Grupos.Any(x => x.Id == g)))
                return (TipoResultado.SolicitudInvalida, "Alguno de los grupos seleccionados no existe.");

            var yaAsignado = _context.Horarios
                .Where(h => h.BloqueExposicionId != idActual && h.GrupoId != null && grupos.Contains(h.GrupoId.Value))
                .Select(h => h.GrupoId!.Value)
                .FirstOrDefault();
            if (yaAsignado != 0)
            {
                var nombre = _context.Grupos.Find(yaAsignado)?.Nombre ?? "Un grupo";
                return (TipoResultado.Conflicto, $"{nombre} ya tiene una exposición asignada en otro bloque.");
            }

            // Mismo día: ni el salón ni un docente pueden estar en dos lugares a la vez.
            var otrosDelDia = _context.Bloques
                .Include(b => b.Horarios)
                .Where(b => b.Id != idActual && b.Fecha == s.Fecha)
                .ToList();

            foreach (var otro in otrosDelDia)
            {
                var mismoSalon = string.Equals(otro.Aula, s.Aula.Trim(), StringComparison.OrdinalIgnoreCase);
                var mismoDocente = new[] { otro.Evaluador1Id, otro.Evaluador2Id }
                    .Any(d => d == s.Evaluador1Id || d == s.Evaluador2Id);
                if (!mismoSalon && !mismoDocente) continue;

                var seSolapan = s.Horarios.Any(h =>
                    otro.Horarios.Any(oh => h.HoraInicio < oh.HoraFin && oh.HoraInicio < h.HoraFin));
                if (!seSolapan) continue;

                return (TipoResultado.Conflicto, mismoSalon
                    ? $"El salón {otro.Aula} ya está ocupado en ese horario."
                    : "Uno de los docentes ya está en otro bloque en ese horario.");
            }

            return null;
        }

        // Convierte entidades a la forma que consume el front end (con nombres y título del artículo).
        private List<BloqueDto> Convertir(IEnumerable<BloqueExposicion> bloques, int? soloGrupoId = null)
        {
            var docentes = _context.Evaluadores.ToDictionary(e => e.Id, e => e.Nombre);
            var grupos = _context.Grupos.Include(g => g.Estudiantes).ToDictionary(g => g.Id);

            var resultado = new List<BloqueDto>();
            foreach (var b in bloques)
            {
                var horarios = new List<HorarioDto>();
                foreach (var h in b.Horarios.OrderBy(x => x.HoraInicio))
                {
                    if (soloGrupoId is not null && h.GrupoId != soloGrupoId) continue;

                    Grupo? grupo = null;
                    if (h.GrupoId is int idGrupo) grupos.TryGetValue(idGrupo, out grupo);

                    horarios.Add(new HorarioDto(h.Id, h.HoraInicio, h.HoraFin, h.GrupoId,
                        grupo?.Nombre, grupo?.ArticuloDeInvestigacion,
                        grupo?.Estudiantes.Select(e => e.Nombre).ToList() ?? new List<string>()));
                }

                resultado.Add(new BloqueDto(b.Id, b.Fecha, b.Aula,
                    b.Evaluador1Id, docentes.GetValueOrDefault(b.Evaluador1Id, "—"),
                    b.Evaluador2Id, docentes.GetValueOrDefault(b.Evaluador2Id, "—"),
                    horarios));
            }
            return resultado;
        }
    }
}
