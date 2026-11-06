if (Sesion.obtener()) {
  window.location.href = 'panel.html';
}

const formulario = document.getElementById('formLogin');
const campoUsuario = document.getElementById('usuario');
const campoContrasena = document.getElementById('contrasena');
const mensajeError = document.getElementById('errorLogin');
const botonLogin = document.getElementById('botonLogin');

formulario.addEventListener('submit', async (evento) => {
  evento.preventDefault();
  mensajeError.hidden = true;

  const nombreUsuario = campoUsuario.value.trim();
  const contrasena = campoContrasena.value;

  if (!nombreUsuario || !contrasena) {
    mensajeError.textContent = 'Ingrese su usuario y contraseña.';
    mensajeError.hidden = false;
    return;
  }

  botonLogin.disabled = true;
  botonLogin.textContent = 'Ingresando…';

  try {
    const datos = await Api.post('api/autenticacion/login', { nombreUsuario, contrasena });
    Sesion.guardar(datos);
    window.location.href = 'panel.html';
  } catch (error) {
    mensajeError.textContent = error.message;
    mensajeError.hidden = false;
    botonLogin.disabled = false;
    botonLogin.textContent = 'Iniciar Sesión →';
  }
});
