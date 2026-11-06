const sesion = Sesion.obtener();

if (!sesion) {
  window.location.replace('index.html');
  throw new Error('No hay una sesión activa.');
}

const contenido = document.getElementById('contenido');
const menu = document.getElementById('menu');
const modal = document.getElementById('modal');
const aviso = document.getElementById('aviso');

const esc = (valor) => String(valor ?? '').replace(/[&<>"']/g, (caracter) => ({
  '&': '&amp;',
  '<': '&lt;',
  '>': '&gt;',
  '"': '&quot;',
  "'": '&#39;'
}[caracter]));

const formatoFecha = (valor) => valor
  ? new Date(valor).toLocaleString('es-CO', { dateStyle: 'medium', timeStyle: 'short' })
  : '—';

const formatoNota = (valor, decimales = 1) => (valor === null || valor === undefined)
  ? '—'
  : Number(valor).toFixed(decimales);

function mostrarAviso(texto, esError = false) {
  aviso.textContent = texto;
  aviso.className = `aviso ${esError ? 'aviso-error' : 'aviso-ok'} visible`;
  clearTimeout(mostrarAviso.temporizador);
  mostrarAviso.temporizador = setTimeout(() => aviso.classList.remove('visible'), 3500);
}

function claseEstado(estado) {
  if (estado === 'Calificada' || estado === 'Consolidado') return 'chip-ok';
  if (estado === 'Faltan evaluadores') return 'chip-alerta';
  return 'chip-pendiente';
}

function chipEstado(estado) {
  return `<span class="chip ${claseEstado(estado)}">${esc(estado)}</span>`;
}

function tarjetaResumen(etiqueta, valor) {
  return `<div class="tarjeta-resumen"><div class="valor">${esc(valor)}</div><div class="etiqueta">${esc(etiqueta)}</div></div>`;
}

function tabla(columnas, filas, accion) {
  if (!filas.length) {
    return '<p class="vacio">No hay registros.</p>';
  }

  const encabezado = columnas.map((c) => `<th>${esc(c.titulo)}</th>`).join('') + (accion ? '<th></th>' : '');
  const cuerpo = filas.map((fila) => {
    const celdas = columnas.map((c) => `<td>${c.formato ? c.formato(fila) : esc(fila[c.campo])}</td>`).join('');
    const celdaAccion = accion ? `<td class="celda-accion">${accion(fila)}</td>` : '';
    return `<tr>${celdas}${celdaAccion}</tr>`;
  }).join('');

  return `<div class="tabla-contenedor"><table><thead><tr>${encabezado}</tr></thead><tbody>${cuerpo}</tbody></table></div>`;
}

function tarjetaEvaluacion(evaluacion) {
  const calificada = evaluacion.estado === 'Calificada';
  return `
    <article class="articulo">
      <h3>${esc(evaluacion.articuloTitulo)}</h3>
      <p class="meta">${esc(evaluacion.articuloAutores)}</p>
      <p class="meta"><strong>${esc(evaluacion.grupoNombre)}</strong> · Orden ${esc(evaluacion.orden)} · ${esc(formatoFecha(evaluacion.fechaExposicion))}</p>
      <p class="meta">Estudiantes: ${esc(evaluacion.estudiantes.join(', ') || '—')}</p>
      <p class="meta">Asesor: ${esc(evaluacion.asesorNombre)}</p>
      <div class="pie">
        <div>
          ${chipEstado(evaluacion.estado)}
          ${calificada ? `<span class="nota-grande">&nbsp; ${esc(formatoNota(evaluacion.nota))}</span>` : ''}
        </div>
        <button type="button" data-calificar="${evaluacion.id}">${calificada ? 'Editar nota' : 'Calificar'}</button>
      </div>
    </article>`;
}

function abrirModalCalificar(evaluacion) {
  modal.innerHTML = `
    <form id="formCalificar" class="modal-contenido">
      <h2>${esc(evaluacion.articuloTitulo)}</h2>
      <p class="meta">${esc(evaluacion.articuloAutores)}</p>
      <p class="resumen-articulo">${esc(evaluacion.articuloResumen)}</p>
      <p class="meta"><strong>${esc(evaluacion.grupoNombre)}</strong> · ${esc(evaluacion.estudiantes.join(', ') || '—')}</p>
      <div>
        <label for="notaEntrada">Nota (0.0 a 5.0)</label>
        <input id="notaEntrada" type="number" min="0" max="5" step="0.1" required value="${evaluacion.nota ?? ''}">
      </div>
      <div>
        <label for="observacionesEntrada">Observaciones</label>
        <textarea id="observacionesEntrada" rows="4" maxlength="500">${esc(evaluacion.observaciones)}</textarea>
      </div>
      <p id="errorModal" class="error" hidden></p>
      <div class="acciones">
        <button type="button" class="secundario" id="cancelarModal">Cancelar</button>
        <button type="submit">Guardar nota</button>
      </div>
    </form>`;

  document.getElementById('cancelarModal').addEventListener('click', () => modal.close());

  document.getElementById('formCalificar').addEventListener('submit', async (evento) => {
    evento.preventDefault();
    const errorModal = document.getElementById('errorModal');
    errorModal.hidden = true;

    try {
      await Api.put(`api/evaluaciones/${evaluacion.id}/calificar`, {
        nota: Number(document.getElementById('notaEntrada').value),
        observaciones: document.getElementById('observacionesEntrada').value
      });
      modal.close();
      mostrarAviso('Nota guardada correctamente.');
      await cargarVista();
    } catch (error) {
      errorModal.textContent = error.message;
      errorModal.hidden = false;
    }
  });

  modal.showModal();
}

async function vistaDocente() {
  const evaluaciones = await Api.get('api/evaluaciones/mis-evaluaciones');
  const calificadas = evaluaciones.filter((e) => e.estado === 'Calificada').length;

  contenido.innerHTML = `
    <section class="resumen">
      ${tarjetaResumen('Artículos asignados', evaluaciones.length)}
      ${tarjetaResumen('Calificados', calificadas)}
      ${tarjetaResumen('Pendientes', evaluaciones.length - calificadas)}
    </section>
    <section class="lista-articulos">
      ${evaluaciones.map(tarjetaEvaluacion).join('') || '<p class="vacio">No tiene artículos asignados.</p>'}
    </section>`;

  contenido.querySelectorAll('[data-calificar]').forEach((boton) => {
    boton.addEventListener('click', () => {
      const evaluacion = evaluaciones.find((e) => e.id === Number(boton.dataset.calificar));
      abrirModalCalificar(evaluacion);
    });
  });
}

async function vistaResultados() {
  const resultados = await Api.get('api/evaluaciones/resultados');

  contenido.innerHTML = `
    <section class="panel">
      <h2>Resultados consolidados por grupo</h2>
      ${tabla([
        { titulo: 'Grupo', campo: 'grupoNombre' },
        { titulo: 'Artículo', campo: 'articuloTitulo' },
        { titulo: 'Evaluadores', formato: (r) => `${esc(r.calificadas)} / ${esc(r.totalEvaluadores)}` },
        { titulo: 'Promedio', formato: (r) => esc(formatoNota(r.promedio, 2)) },
        { titulo: 'Estado', formato: (r) => chipEstado(r.estado) }
      ], resultados)}
    </section>`;
}

async function vistaEvaluacionesAdmin() {
  const [evaluaciones, grupos, docentes] = await Promise.all([
    Api.get('api/evaluaciones'),
    Api.get('api/grupos'),
    Api.get('api/docentes')
  ]);

  const opcionesGrupos = grupos.map((g) => `<option value="${esc(g.id)}">${esc(g.nombre)} — ${esc(g.articuloTitulo)}</option>`).join('');
  const opcionesDocentes = docentes.map((d) => `<option value="${esc(d.id)}">${esc(d.nombre)}</option>`).join('');

  contenido.innerHTML = `
    <section class="panel">
      <h2>Asignar evaluador a un grupo</h2>
      <form id="formAsignar" class="formulario">
        <div>
          <label for="grupoAsignar">Grupo</label>
          <select id="grupoAsignar" required><option value="">Seleccione…</option>${opcionesGrupos}</select>
        </div>
        <div>
          <label for="docenteAsignar">Docente evaluador</label>
          <select id="docenteAsignar" required><option value="">Seleccione…</option>${opcionesDocentes}</select>
        </div>
        <div class="acciones"><button type="submit">Asignar</button></div>
      </form>
    </section>
    <section class="panel">
      <h2>Evaluaciones asignadas</h2>
      ${tabla([
        { titulo: 'Grupo', campo: 'grupoNombre' },
        { titulo: 'Artículo', campo: 'articuloTitulo' },
        { titulo: 'Evaluador', campo: 'evaluadorNombre' },
        { titulo: 'Nota', formato: (e) => esc(formatoNota(e.nota)) },
        { titulo: 'Estado', formato: (e) => chipEstado(e.estado) }
      ], evaluaciones, (e) => e.estado === 'Pendiente' ? `<button type="button" class="peligro" data-eliminar="${esc(e.id)}">Quitar</button>` : '')}
    </section>`;

  document.getElementById('formAsignar').addEventListener('submit', async (evento) => {
    evento.preventDefault();
    try {
      await Api.post('api/evaluaciones', {
        grupoId: Number(document.getElementById('grupoAsignar').value),
        docenteId: Number(document.getElementById('docenteAsignar').value)
      });
      mostrarAviso('Evaluador asignado.');
      await cargarVista();
    } catch (error) {
      mostrarAviso(error.message, true);
    }
  });

  contenido.querySelectorAll('[data-eliminar]').forEach((boton) => {
    boton.addEventListener('click', async () => {
      if (!confirm('¿Quitar esta asignación?')) return;
      try {
        await Api.del(`api/evaluaciones/${boton.dataset.eliminar}`);
        mostrarAviso('Asignación eliminada.');
        await cargarVista();
      } catch (error) {
        mostrarAviso(error.message, true);
      }
    });
  });
}

const opcionesDesde = async (url, valor, texto, filtro = () => true) => {
  const datos = await Api.get(url);
  return datos.filter(filtro).map((d) => ({ valor: d[valor], texto: typeof texto === 'function' ? texto(d) : d[texto] }));
};

const catalogos = {
  grupos: {
    titulo: 'Grupos de exposición',
    singular: 'grupo de exposición',
    url: 'api/grupos',
    campos: [
      { nombre: 'nombre', etiqueta: 'Nombre del grupo', tipo: 'text' },
      { nombre: 'articuloId', etiqueta: 'Artículo científico', tipo: 'select', opciones: () => opcionesDesde('api/articulos', 'id', 'titulo') },
      { nombre: 'asesorId', etiqueta: 'Docente asesor', tipo: 'select', opciones: () => opcionesDesde('api/docentes', 'id', 'nombre') },
      { nombre: 'orden', etiqueta: 'Orden de exposición', tipo: 'number', min: 1 },
      { nombre: 'fechaExposicion', etiqueta: 'Fecha y hora', tipo: 'datetime-local', opcional: true },
      { nombre: 'estudianteIds', etiqueta: 'Estudiantes sin grupo', tipo: 'multiselect', opciones: () => opcionesDesde('api/estudiantes', 'id', (e) => `${e.codigo} · ${e.nombre}`, (e) => !e.grupoId) }
    ],
    columnas: [
      { titulo: 'Orden', campo: 'orden' },
      { titulo: 'Grupo', campo: 'nombre' },
      { titulo: 'Artículo', campo: 'articuloTitulo' },
      { titulo: 'Asesor', campo: 'asesorNombre' },
      { titulo: 'Fecha', formato: (g) => esc(formatoFecha(g.fechaExposicion)) },
      { titulo: 'Estudiantes', formato: (g) => esc(g.estudiantes.join(', ') || '—') },
      { titulo: 'Evaluadores', campo: 'totalEvaluadores' }
    ]
  },
  articulos: {
    titulo: 'Artículos científicos',
    singular: 'artículo científico',
    url: 'api/articulos',
    campos: [
      { nombre: 'titulo', etiqueta: 'Título', tipo: 'text' },
      { nombre: 'autores', etiqueta: 'Autores', tipo: 'text' },
      { nombre: 'revista', etiqueta: 'Revista', tipo: 'text' },
      { nombre: 'anio', etiqueta: 'Año', tipo: 'number', min: 1900 },
      { nombre: 'resumen', etiqueta: 'Resumen', tipo: 'textarea' }
    ],
    columnas: [
      { titulo: 'Título', campo: 'titulo' },
      { titulo: 'Autores', campo: 'autores' },
      { titulo: 'Revista', campo: 'revista' },
      { titulo: 'Año', campo: 'anio' }
    ]
  },
  docentes: {
    titulo: 'Docentes',
    singular: 'docente',
    url: 'api/docentes',
    campos: [
      { nombre: 'nombre', etiqueta: 'Nombre completo', tipo: 'text' },
      { nombre: 'correo', etiqueta: 'Correo institucional', tipo: 'email' },
      { nombre: 'departamento', etiqueta: 'Departamento', tipo: 'text' },
      { nombre: 'nombreUsuario', etiqueta: 'Usuario de acceso', tipo: 'text' },
      { nombre: 'contrasena', etiqueta: 'Contraseña (mínimo 6)', tipo: 'password' }
    ],
    columnas: [
      { titulo: 'Nombre', campo: 'nombre' },
      { titulo: 'Correo', campo: 'correo' },
      { titulo: 'Departamento', campo: 'departamento' },
      { titulo: 'Usuario', formato: (d) => esc(d.nombreUsuario || '—') }
    ]
  },
  estudiantes: {
    titulo: 'Estudiantes',
    singular: 'estudiante',
    url: 'api/estudiantes',
    campos: [
      { nombre: 'nombre', etiqueta: 'Nombre completo', tipo: 'text' },
      { nombre: 'codigo', etiqueta: 'Código', tipo: 'text' }
    ],
    columnas: [
      { titulo: 'Código', campo: 'codigo' },
      { titulo: 'Nombre', campo: 'nombre' },
      { titulo: 'Grupo', formato: (e) => esc(e.grupoNombre || '—') }
    ]
  }
};

async function campoHtml(campo) {
  const id = `campo-${campo.nombre}`;
  let control;

  if (campo.tipo === 'select' || campo.tipo === 'multiselect') {
    const opciones = await campo.opciones();
    const lista = opciones.map((o) => `<option value="${esc(o.valor)}">${esc(o.texto)}</option>`).join('');
    control = campo.tipo === 'select'
      ? `<select id="${id}" required><option value="">Seleccione…</option>${lista}</select>`
      : `<select id="${id}" multiple size="4">${lista}</select>`;
  } else if (campo.tipo === 'textarea') {
    control = `<textarea id="${id}" rows="3" required></textarea>`;
  } else {
    const minimo = campo.min !== undefined ? `min="${campo.min}"` : '';
    control = `<input id="${id}" type="${campo.tipo}" ${minimo} ${campo.opcional ? '' : 'required'}>`;
  }

  return `<div><label for="${id}">${esc(campo.etiqueta)}</label>${control}</div>`;
}

function valorCampo(campo) {
  const control = document.getElementById(`campo-${campo.nombre}`);

  switch (campo.tipo) {
    case 'number':
    case 'select':
      return Number(control.value);
    case 'multiselect':
      return Array.from(control.selectedOptions).map((opcion) => Number(opcion.value));
    case 'datetime-local':
      return control.value || null;
    default:
      return control.value.trim();
  }
}

async function vistaCatalogo(clave) {
  const configuracion = catalogos[clave];
  const [filas, camposFormulario] = await Promise.all([
    Api.get(configuracion.url),
    Promise.all(configuracion.campos.map(campoHtml))
  ]);

  contenido.innerHTML = `
    <section class="panel">
      <h2>Registrar ${esc(configuracion.singular)}</h2>
      <form id="formCatalogo" class="formulario">
        ${camposFormulario.join('')}
        <div class="acciones"><button type="submit">Guardar</button></div>
      </form>
    </section>
    <section class="panel">
      <h2>${esc(configuracion.titulo)}</h2>
      ${tabla(configuracion.columnas, filas, (fila) => `<button type="button" class="peligro" data-eliminar="${esc(fila.id)}">Eliminar</button>`)}
    </section>`;

  document.getElementById('formCatalogo').addEventListener('submit', async (evento) => {
    evento.preventDefault();
    const cuerpo = {};
    configuracion.campos.forEach((campo) => {
      cuerpo[campo.nombre] = valorCampo(campo);
    });

    try {
      await Api.post(configuracion.url, cuerpo);
      mostrarAviso('Registro guardado.');
      await cargarVista();
    } catch (error) {
      mostrarAviso(error.message, true);
    }
  });

  contenido.querySelectorAll('[data-eliminar]').forEach((boton) => {
    boton.addEventListener('click', async () => {
      if (!confirm('¿Eliminar este registro?')) return;
      try {
        await Api.del(`${configuracion.url}/${boton.dataset.eliminar}`);
        mostrarAviso('Registro eliminado.');
        await cargarVista();
      } catch (error) {
        mostrarAviso(error.message, true);
      }
    });
  });
}

const pestanasAdmin = [
  { clave: 'resultados', titulo: 'Resultados', vista: vistaResultados },
  { clave: 'evaluaciones', titulo: 'Evaluaciones', vista: vistaEvaluacionesAdmin },
  { clave: 'grupos', titulo: 'Grupos', vista: () => vistaCatalogo('grupos') },
  { clave: 'articulos', titulo: 'Artículos', vista: () => vistaCatalogo('articulos') },
  { clave: 'docentes', titulo: 'Docentes', vista: () => vistaCatalogo('docentes') },
  { clave: 'estudiantes', titulo: 'Estudiantes', vista: () => vistaCatalogo('estudiantes') }
];

let pestanaActual = 'resultados';

function pintarMenu() {
  menu.hidden = false;
  menu.innerHTML = pestanasAdmin
    .map((p) => `<button type="button" data-pestana="${p.clave}" class="${p.clave === pestanaActual ? 'activo' : ''}">${esc(p.titulo)}</button>`)
    .join('');

  menu.querySelectorAll('[data-pestana]').forEach((boton) => {
    boton.addEventListener('click', () => {
      pestanaActual = boton.dataset.pestana;
      cargarVista();
    });
  });
}

async function cargarVista() {
  try {
    if (sesion.rol === 'Admin') {
      pintarMenu();
      const pestana = pestanasAdmin.find((p) => p.clave === pestanaActual);
      await pestana.vista();
    } else {
      await vistaDocente();
    }
  } catch (error) {
    contenido.innerHTML = `<p class="error">${esc(error.message)}</p>`;
  }
}

document.getElementById('nombreUsuario').textContent = sesion.nombreDocente || sesion.nombreUsuario;
document.getElementById('rolUsuario').textContent = sesion.rol === 'Admin' ? 'Administrador' : 'Docente evaluador';
document.getElementById('botonSalir').addEventListener('click', () => Sesion.cerrar());

cargarVista();
