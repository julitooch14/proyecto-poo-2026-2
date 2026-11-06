const Sesion = {
  claveAlmacenamiento: 'jeac_sesion',

  guardar(datos) {
    sessionStorage.setItem(this.claveAlmacenamiento, JSON.stringify(datos));
  },

  obtener() {
    const texto = sessionStorage.getItem(this.claveAlmacenamiento);
    return texto ? JSON.parse(texto) : null;
  },

  cerrar() {
    sessionStorage.removeItem(this.claveAlmacenamiento);
    window.location.href = 'index.html';
  }
};

async function solicitar(metodo, url, cuerpo) {
  const sesion = Sesion.obtener();
  const cabeceras = { 'Content-Type': 'application/json' };

  if (sesion) {
    cabeceras['Authorization'] = `Bearer ${sesion.token}`;
  }

  const respuesta = await fetch(url, {
    method: metodo,
    headers: cabeceras,
    body: cuerpo === undefined ? undefined : JSON.stringify(cuerpo)
  });

  if (respuesta.status === 401 && sesion) {
    Sesion.cerrar();
    throw new Error('La sesión expiró. Inicie sesión nuevamente.');
  }

  if (respuesta.status === 204) {
    return null;
  }

  const texto = await respuesta.text();
  const datos = texto ? JSON.parse(texto) : null;

  if (!respuesta.ok) {
    if (datos && datos.mensaje) {
      throw new Error(datos.mensaje);
    }
    if (datos && datos.errors) {
      throw new Error(Object.values(datos.errors).flat().join(' '));
    }
    if (respuesta.status === 403) {
      throw new Error('No tiene permisos para realizar esta acción.');
    }
    throw new Error('Ocurrió un error inesperado.');
  }

  return datos;
}

const Api = {
  get: (url) => solicitar('GET', url),
  post: (url, cuerpo) => solicitar('POST', url, cuerpo),
  put: (url, cuerpo) => solicitar('PUT', url, cuerpo),
  del: (url) => solicitar('DELETE', url)
};
