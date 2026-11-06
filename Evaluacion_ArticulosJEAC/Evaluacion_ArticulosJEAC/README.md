# Evaluación de artículos científicos JEAC (Módulo 5)

## Cómo abrirlo en Visual Studio Community
1. Descomprima el .zip.
2. Abra `Evaluacion_ArticulosJEAC.sln` (Archivo > Abrir > Proyecto o solución).
3. Clic derecho en `Evaluacion_ArticulosJEAC.Api` > Establecer como proyecto de inicio.
4. Presione F5. Visual Studio restaura los paquetes NuGet solo (necesita internet la primera vez).
5. Se abre http://localhost:5080 con el inicio de sesión. Swagger: http://localhost:5080/swagger

Requiere Visual Studio 2022 o superior con la carga de trabajo "ASP.NET y desarrollo web" y el SDK de .NET 8.

## Credenciales de prueba
| Usuario | Contraseña | Rol |
|---|---|---|
| admin | admin123 | Administrador |
| user | user123 | Docente |
| maria | maria123 | Docente |
| luis | luis123 | Docente |

## Pruebas unitarias
Menú Prueba > Ejecutar todas las pruebas.

## Estructura
- `Evaluacion_ArticulosJEAC.Api`: Models, Dtos, Data, Repositories, Services, Controllers, Security, Middleware, Exceptions, Mappers y `wwwroot` (interfaz).
- `Evaluacion_ArticulosJEAC.Tests`: pruebas xUnit.
- `docs/diagramas`: diagramas UML (PNG y fuente PlantUML).
