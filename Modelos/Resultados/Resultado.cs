namespace Modelos.Resultados
{
    // Tipos de respuesta que un servicio le puede devolver al controlador.
    // Así el servicio no necesita conocer nada de HTTP (BadRequest, NotFound, etc.).
    public enum TipoResultado
    {
        Exito,
        SolicitudInvalida,
        NoEncontrado,
        Conflicto,
        NoAutorizado
    }

    public class Resultado<T>
    {
        public TipoResultado Tipo { get; private set; }
        public T? Valor { get; private set; }
        public string? Mensaje { get; private set; }
        public bool Exitoso => Tipo == TipoResultado.Exito;

        public static Resultado<T> Exito(T valor) =>
            new() { Tipo = TipoResultado.Exito, Valor = valor };

        public static Resultado<T> Falla(TipoResultado tipo, string mensaje) =>
            new() { Tipo = tipo, Mensaje = mensaje };
    }
}
