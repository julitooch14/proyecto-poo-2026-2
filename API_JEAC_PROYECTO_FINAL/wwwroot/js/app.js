(() => {
  'use strict';

  /* ================= Utilidades ================= */
  const $ = (sel) => document.querySelector(sel);
  const esc = (t) => String(t ?? '').replace(/[&<>"']/g, (c) =>
    ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
  const hhmm = (t) => (t || '').slice(0, 5);
  const conSegundos = (t) => (t.length === 5 ? t + ':00' : t);
  const aMinutos = (t) => { const [h, m] = t.split(':').map(Number); return h * 60 + m; };
  const deMinutos = (n) => String(Math.floor(n / 60)).padStart(2, '0') + ':' + String(n % 60).padStart(2, '0');
  const fechaDe = (iso) => { const [y, m, d] = iso.split('-').map(Number); return new Date(y, m - 1, d); };
  const fechaCorta = (iso) => fechaDe(iso).toLocaleDateString('es-CO', { weekday: 'short', day: 'numeric', month: 'short' });
  const fechaLarga = (iso) => fechaDe(iso).toLocaleDateString('es-CO', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' });
  const hoyISO = () => { const d = new Date(); return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`; };
  const COLORES = ['#1d5c48', '#c28a00', '#2f6fb5', '#a8457a', '#6a5acd', '#c2562a'];

  function toast(mensaje, tipo = 'ok') {
    const t = document.createElement('div');
    t.className = 'toast ' + (tipo === 'mal' ? 'mal' : '');
    t.textContent = mensaje;
    $('#toasts').appendChild(t);
    setTimeout(() => t.remove(), 5000);
  }

  /* ================= Comunicación con la API ================= */
  let sesion = JSON.parse(sessionStorage.getItem('jeac') || 'null');

  async function api(ruta, metodo = 'GET', cuerpo) {
    const res = await fetch('/api/' + ruta, {
      method: metodo,
      headers: {
        'Content-Type': 'application/json',
        ...(sesion ? { Authorization: 'Bearer ' + sesion.token } : {})
      },
      body: cuerpo ? JSON.stringify(cuerpo) : undefined
    });
    if (res.status === 401 && sesion) {
      cerrarSesion('Tu sesión expiró. Ingresa de nuevo.');
      throw new Error('Sesión expirada');
    }
    const datos = await res.json().catch(() => null);
    if (!res.ok) {
      const primerError = datos?.errors ? Object.values(datos.errors)[0]?.[0] : null;
      const error = new Error(datos?.mensaje || primerError || `No se pudo completar la acción (${res.status}).`);
      error.status = res.status;
      throw error;
    }
    return datos;
  }

  /* ================= Estado ================= */
  const estado = { bloques: [], grupos: [], docentes: [], filtroAula: '' };
  const ed = { id: null, filas: [] }; // bloque que se está armando en el editor

  /* ================= Sesión / ingreso ================= */
  function cerrarSesion(mensaje) {
    sesion = null;
    sessionStorage.removeItem('jeac');
    $('#vista-app').hidden = true;
    $('#vista-login').hidden = false;
    $('#contrasena').value = '';
    const err = $('#login-error');
    err.hidden = !mensaje;
    err.textContent = mensaje || '';
  }

  async function iniciarApp() {
    $('#vista-login').hidden = true;
    $('#vista-app').hidden = false;
    $('#top-rol').textContent = sesion.rol;
    $('#top-nombre').textContent = sesion.nombre;
    $('#contenido').innerHTML = '<p class="vacio-estado">Cargando…</p>';
    try {
      if (sesion.rol === 'Administrador') await vistaAdmin();
      else if (sesion.rol === 'Docente') await vistaDocente();
      else await vistaEstudiante();
    } catch (e) {
      if (sesion) toast(e.message, 'mal');
    }
  }

  $('#form-login').addEventListener('submit', async (e) => {
    e.preventDefault();
    const boton = $('#login-btn');
    boton.disabled = true;
    $('#login-error').hidden = true;
    try {
      const r = await api('Autenticacion/login', 'POST', {
        usuario: $('#usuario').value,
        contrasena: $('#contrasena').value
      });
      sesion = { token: r.token, rol: r.rol, nombre: r.nombre };
      sessionStorage.setItem('jeac', JSON.stringify(sesion));
      await iniciarApp();
    } catch (err) {
      $('#login-error').textContent = err.message;
      $('#login-error').hidden = false;
    } finally {
      boton.disabled = false;
    }
  });

  document.querySelectorAll('.demo button').forEach((b) =>
    b.addEventListener('click', () => {
      $('#usuario').value = b.dataset.u;
      $('#contrasena').value = '1234';
      $('#login-btn').focus();
    }));

  $('#btn-salir').addEventListener('click', () => cerrarSesion());

  /* ================= Cronograma completo (admin y docente) ================= */
  async function cargarBloques() {
    estado.bloques = await api('Bloques');
  }

  function htmlCronograma() {
    return `
      <section class="panel">
        <div class="panel-cab">
          <h2>Cronograma completo</h2>
          <div class="filtros">
            <select id="filtro-aula" aria-label="Filtrar por salón"></select>
            <button class="btn" id="btn-imprimir" type="button">Imprimir</button>
          </div>
        </div>
        <p class="resumen" id="resumen"></p>
        <div class="tabla-wrap">
          <table class="tabla crono">
            <thead><tr>
              <th>Fecha</th><th>Salón</th><th>Horario</th><th>Artículo de investigación</th><th>Grupo</th><th>Docentes</th>
            </tr></thead>
            <tbody id="crono-cuerpo"></tbody>
          </table>
        </div>
      </section>`;
  }

  function pintarCronograma(esAdmin) {
    // filtro por salón
    const salones = [...new Set(estado.bloques.map((b) => b.aula))].sort();
    if (!salones.includes(estado.filtroAula)) estado.filtroAula = '';
    const filtro = $('#filtro-aula');
    filtro.innerHTML = '<option value="">Todos los salones</option>' +
      salones.map((s) => `<option ${s === estado.filtroAula ? 'selected' : ''}>${esc(s)}</option>`).join('');

    const filas = [];
    estado.bloques.forEach((b) => {
      if (estado.filtroAula && b.aula !== estado.filtroAula) return;
      b.horarios.forEach((h) => filas.push({ b, h }));
    });

    const asignados = filas.filter((f) => f.h.grupoId).length;
    $('#resumen').textContent = `${filas.length} horarios · ${asignados} con grupo asignado`;

    const vacio = esAdmin
      ? 'Todavía no hay bloques guardados. Arma el primero en el panel de la izquierda.'
      : 'Todavía no hay exposiciones programadas.';

    $('#crono-cuerpo').innerHTML = filas.length ? filas.map(({ b, h }) => `
      <tr class="${esAdmin ? 'click' : ''} ${b.id === ed.id ? 'sel' : ''}" data-id="${b.id}"
          style="--bloque:${COLORES[b.id % COLORES.length]}" ${esAdmin ? 'title="Clic para editar este bloque"' : ''}>
        <td class="fecha">${esc(fechaCorta(b.fecha))}</td>
        <td>${esc(b.aula)}</td>
        <td class="hora">${hhmm(h.horaInicio)} – ${hhmm(h.horaFin)}</td>
        <td class="articulo">${h.articulo ? esc(h.articulo) : '<span class="vacio">—</span>'}</td>
        <td>${h.grupoNombre
          ? `<strong>${esc(h.grupoNombre)}</strong><br><small>${esc(h.estudiantes.join(' y '))}</small>`
          : '<span class="vacio">Libre</span>'}</td>
        <td>${esc(b.evaluador1)}<br>${esc(b.evaluador2)}</td>
      </tr>`).join('')
      : `<tr><td colspan="6" class="vacio">${vacio}</td></tr>`;
  }

  function enlazarCronograma(esAdmin) {
    $('#filtro-aula').addEventListener('change', (e) => {
      estado.filtroAula = e.target.value;
      pintarCronograma(esAdmin);
    });
    $('#btn-imprimir').addEventListener('click', () => window.print());
    if (esAdmin) {
      $('#crono-cuerpo').addEventListener('click', (e) => {
        const fila = e.target.closest('tr[data-id]');
        if (fila) cargarBloque(Number(fila.dataset.id));
      });
    }
  }

  /* ================= Vista docente ================= */
  async function vistaDocente() {
    await cargarBloques();
    $('#contenido').innerHTML = htmlCronograma();
    pintarCronograma(false);
    enlazarCronograma(false);
  }

  /* ================= Vista estudiante ================= */
  async function vistaEstudiante() {
    try {
      const b = await api('Bloques/MiHorario');
      const h = b.horarios[0];
      $('#contenido').innerHTML = `
        <section class="mi-horario">
          <article class="ticket">
            <div class="ticket-cab">
              <h2>Tu exposición</h2>
              <p>${esc(fechaLarga(b.fecha))}</p>
              <span class="ticket-hora">${hhmm(h.horaInicio)} – ${hhmm(h.horaFin)}</span>
            </div>
            <dl>
              <dt>Artículo</dt><dd class="titulo">${esc(h.articulo)}</dd>
              <dt>Grupo</dt><dd>${esc(h.grupoNombre)}: ${esc(h.estudiantes.join(' y '))}</dd>
              <dt>Salón</dt><dd>${esc(b.aula)}</dd>
              <dt>Evaluadores</dt><dd>${esc(b.evaluador1)} y ${esc(b.evaluador2)}</dd>
            </dl>
          </article>
        </section>`;
    } catch (e) {
      if (e.status !== 404) throw e;
      $('#contenido').innerHTML = `<p class="vacio-estado"><strong>${esc(e.message)}</strong><br>Cuando el administrador te asigne un horario aparecerá aquí.</p>`;
    }
  }

  /* ================= Vista administrador ================= */
  async function vistaAdmin() {
    const [, grupos, docentes] = await Promise.all([
      cargarBloques(), api('Catalogo/Grupos'), api('Catalogo/Evaluadores')
    ]);
    estado.grupos = grupos;
    estado.docentes = docentes;

    $('#contenido').innerHTML = `<div class="dos-col">${htmlEditor()}${htmlCronograma()}</div>`;
    enlazarEditor();
    enlazarCronograma(true);
    nuevoBloque(hoyISO());
    pintarCronograma(true);
  }

  function htmlEditor() {
    return `
      <section class="panel editor" id="panel-editor">
        <div class="panel-cab">
          <h2>Bloque de exposiciones</h2>
          <span class="pill" id="estado-editor">Nuevo bloque</span>
        </div>

        <div class="campos">
          <label>Fecha<input type="date" id="ed-fecha"></label>
          <label>Salón<input id="ed-aula" maxlength="40" placeholder="Ej. Aula 204"></label>
          <label>Docente 1<select id="ed-ev1"></select></label>
          <label>Docente 2<select id="ed-ev2"></select></label>
        </div>

        <div class="generador">
          <label>Primer horario<input type="time" id="gen-inicio" value="08:00"></label>
          <label>Duración (min)<input type="number" id="gen-dur" value="30" min="5" max="180"></label>
          <label>Cantidad<input type="number" id="gen-cant" value="6" min="1" max="20"></label>
          <button class="btn" type="button" id="gen-btn">Generar horarios</button>
        </div>

        <div class="tabla-wrap">
          <table class="tabla">
            <thead><tr><th>Horario</th><th>Artículo de investigación</th><th>Grupo</th><th></th></tr></thead>
            <tbody id="editor-cuerpo"></tbody>
          </table>
        </div>
        <p class="nota" id="sin-asignar"></p>

        <div class="acciones">
          <button class="btn primario" type="button" id="btn-guardar">Guardar bloque</button>
          <button class="btn" type="button" id="btn-agregar">Agregar horario</button>
          <button class="btn" type="button" id="btn-nuevo">Nuevo bloque</button>
          <button class="btn peligro derecha" type="button" id="btn-eliminar" hidden>Eliminar bloque</button>
        </div>
      </section>`;
  }

  function enlazarEditor() {
    $('#gen-btn').addEventListener('click', generarFilas);
    $('#btn-agregar').addEventListener('click', agregarFila);
    $('#btn-guardar').addEventListener('click', guardarBloque);
    $('#btn-nuevo').addEventListener('click', () => { nuevoBloque($('#ed-fecha').value); pintarCronograma(true); });
    $('#btn-eliminar').addEventListener('click', eliminarBloque);
    $('#ed-ev1').addEventListener('change', () => llenarDocentes());
    $('#ed-ev2').addEventListener('change', () => llenarDocentes());

    const cuerpo = $('#editor-cuerpo');
    cuerpo.addEventListener('change', (e) => {
      const i = Number(e.target.dataset.i);
      if (e.target.matches('.sel-grupo')) {
        ed.filas[i].grupoId = e.target.value ? Number(e.target.value) : null;
        pintarFilas();
      } else if (e.target.matches('input[type=time]')) {
        ed.filas[i][e.target.dataset.k] = e.target.value;
      }
    });
    cuerpo.addEventListener('click', (e) => {
      const quitar = e.target.closest('[data-quitar]');
      if (!quitar) return;
      ed.filas.splice(Number(quitar.dataset.quitar), 1);
      pintarFilas();
    });
  }

  // Los dos selectores de docentes no pueden tener el mismo docente.
  function llenarDocentes(v1 = $('#ed-ev1').value, v2 = $('#ed-ev2').value) {
    const opciones = (excluir, actual) => '<option value="">Selecciona…</option>' +
      estado.docentes.filter((d) => String(d.id) !== excluir).map((d) =>
        `<option value="${d.id}" ${String(d.id) === actual ? 'selected' : ''}>${esc(d.nombre)}</option>`).join('');
    $('#ed-ev1').innerHTML = opciones(v2, v1);
    $('#ed-ev2').innerHTML = opciones(v1, v2);
  }

  function estadoEditor() {
    const pill = $('#estado-editor');
    pill.textContent = ed.id ? `Editando bloque #${ed.id}` : 'Nuevo bloque';
    pill.classList.toggle('editando', !!ed.id);
    $('#btn-eliminar').hidden = !ed.id;
  }

  function nuevoBloque(fecha) {
    ed.id = null;
    $('#ed-fecha').value = fecha || hoyISO();
    $('#ed-aula').value = '';
    llenarDocentes('', '');
    ed.filas = [];
    estadoEditor();
    generarFilas(true);
  }

  function cargarBloque(id) {
    const b = estado.bloques.find((x) => x.id === id);
    if (!b) return;
    ed.id = b.id;
    $('#ed-fecha').value = b.fecha;
    $('#ed-aula').value = b.aula;
    llenarDocentes(String(b.evaluador1Id), String(b.evaluador2Id));
    ed.filas = b.horarios.map((h) => ({ inicio: hhmm(h.horaInicio), fin: hhmm(h.horaFin), grupoId: h.grupoId }));
    estadoEditor();
    pintarFilas();
    pintarCronograma(true);
    if (window.innerWidth < 1050) $('#panel-editor').scrollIntoView({ behavior: 'smooth' });
  }

  function generarFilas(sinPreguntar) {
    const inicio = $('#gen-inicio').value;
    const duracion = Number($('#gen-dur').value);
    const cantidad = Number($('#gen-cant').value);
    if (!inicio || !(duracion > 0) || !(cantidad > 0)) {
      toast('Completa el primer horario, la duración y la cantidad.', 'mal');
      return;
    }
    if (aMinutos(inicio) + duracion * cantidad > 24 * 60) {
      toast('Los horarios se pasan de la medianoche. Reduce la cantidad o la duración.', 'mal');
      return;
    }
    if (sinPreguntar !== true && ed.filas.some((f) => f.grupoId) &&
        !confirm('Se reemplazarán los horarios actuales y se quitarán los grupos asignados. ¿Continuar?')) return;
    const base = aMinutos(inicio);
    ed.filas = Array.from({ length: cantidad }, (_, i) => ({
      inicio: deMinutos(base + i * duracion),
      fin: deMinutos(base + (i + 1) * duracion),
      grupoId: null
    }));
    pintarFilas();
  }

  function agregarFila() {
    const duracion = Number($('#gen-dur').value) || 30;
    const ultimo = ed.filas[ed.filas.length - 1];
    const inicio = ultimo ? ultimo.fin : ($('#gen-inicio').value || '08:00');
    if (!inicio || aMinutos(inicio) + duracion > 24 * 60) {
      toast('No cabe otro horario ese día.', 'mal');
      return;
    }
    ed.filas.push({ inicio, fin: deMinutos(aMinutos(inicio) + duracion), grupoId: null });
    pintarFilas();
  }

  function pintarFilas() {
    const enEditor = new Set(ed.filas.map((f) => f.grupoId).filter(Boolean));
    const enOtrosBloques = new Set(estado.bloques.filter((b) => b.id !== ed.id)
      .flatMap((b) => b.horarios.map((h) => h.grupoId)).filter(Boolean));

    $('#editor-cuerpo').innerHTML = ed.filas.length ? ed.filas.map((f, i) => {
      const grupo = estado.grupos.find((g) => g.id === f.grupoId);
      const opciones = estado.grupos
        .filter((g) => g.id === f.grupoId || (!enEditor.has(g.id) && !enOtrosBloques.has(g.id)))
        .map((g) => `<option value="${g.id}" ${g.id === f.grupoId ? 'selected' : ''}>${esc(g.nombre)} · ${esc(g.estudiantes.join(' y '))}</option>`)
        .join('');
      return `
        <tr>
          <td class="hora">
            <input type="time" data-i="${i}" data-k="inicio" value="${f.inicio}" aria-label="Hora de inicio">
            <input type="time" data-i="${i}" data-k="fin" value="${f.fin}" aria-label="Hora de fin">
          </td>
          <td class="articulo">${grupo ? esc(grupo.articulo) : '<span class="vacio">Elige un grupo</span>'}</td>
          <td><select class="sel-grupo" data-i="${i}" aria-label="Grupo del horario ${i + 1}">
            <option value="">— Libre —</option>${opciones}</select></td>
          <td><button type="button" class="icono" data-quitar="${i}" title="Quitar este horario" aria-label="Quitar este horario">×</button></td>
        </tr>`;
    }).join('') : '<tr><td colspan="4" class="vacio">Sin horarios. Genera una serie o agrega uno.</td></tr>';

    const usados = new Set([...enEditor, ...enOtrosBloques]);
    const libres = estado.grupos.filter((g) => !usados.has(g.id)).map((g) => g.nombre);
    $('#sin-asignar').textContent = libres.length
      ? `Grupos sin horario: ${libres.join(', ')}.`
      : 'Todos los grupos ya tienen horario.';
  }

  async function guardarBloque() {
    const fecha = $('#ed-fecha').value;
    const aula = $('#ed-aula').value.trim();
    const ev1 = $('#ed-ev1').value;
    const ev2 = $('#ed-ev2').value;

    if (!fecha) return toast('Elige la fecha del bloque.', 'mal');
    if (!aula) return toast('Escribe el salón del bloque.', 'mal');
    if (!ev1 || !ev2) return toast('Elige los dos docentes del bloque.', 'mal');
    if (!ed.filas.length) return toast('Agrega al menos un horario.', 'mal');
    if (ed.filas.some((f) => !f.inicio || !f.fin)) return toast('Todos los horarios necesitan hora de inicio y de fin.', 'mal');

    const cuerpo = {
      fecha, aula,
      evaluador1Id: Number(ev1),
      evaluador2Id: Number(ev2),
      horarios: ed.filas.map((f) => ({
        horaInicio: conSegundos(f.inicio),
        horaFin: conSegundos(f.fin),
        grupoId: f.grupoId
      }))
    };

    const boton = $('#btn-guardar');
    boton.disabled = true;
    try {
      const editando = ed.id !== null;
      if (editando) await api('Bloques/' + ed.id, 'PUT', cuerpo);
      else await api('Bloques', 'POST', cuerpo);
      await cargarBloques();
      toast(editando ? `Bloque de ${aula} actualizado.` : `Bloque guardado en ${aula}. Ya puedes armar otro.`);
      nuevoBloque(fecha);
      pintarCronograma(true);
    } catch (e) {
      if (sesion) toast(e.message, 'mal');
    } finally {
      boton.disabled = false;
    }
  }

  async function eliminarBloque() {
    if (ed.id === null || !confirm('¿Eliminar este bloque y todos sus horarios?')) return;
    try {
      await api('Bloques/' + ed.id, 'DELETE');
      await cargarBloques();
      toast('Bloque eliminado.');
      nuevoBloque($('#ed-fecha').value);
      pintarCronograma(true);
    } catch (e) {
      if (sesion) toast(e.message, 'mal');
    }
  }

  /* ================= Arranque ================= */
  if (sesion) iniciarApp();
})();
