namespace Evaluacion_ArticulosJEAC.Api.Security;

public interface IHasheadorContrasenas
{
    string Hashear(string contrasena);
    bool Verificar(string contrasena, string hashAlmacenado);
}
