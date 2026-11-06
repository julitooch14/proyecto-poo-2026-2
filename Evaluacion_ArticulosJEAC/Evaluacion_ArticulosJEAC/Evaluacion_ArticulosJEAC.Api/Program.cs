using System.Text;
using Evaluacion_ArticulosJEAC.Api.Data;
using Evaluacion_ArticulosJEAC.Api.Middleware;
using Evaluacion_ArticulosJEAC.Api.Repositories;
using Evaluacion_ArticulosJEAC.Api.Security;
using Evaluacion_ArticulosJEAC.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var seccionJwt = builder.Configuration.GetSection("Jwt");
var opcionesJwt = seccionJwt.Get<OpcionesJwt>()
    ?? throw new InvalidOperationException("Falta la sección Jwt en appsettings.json.");

builder.Services.Configure<OpcionesJwt>(seccionJwt);

builder.Services.AddDbContext<AppDbContext>(opciones => opciones.UseInMemoryDatabase("EvaluacionJeacDb"));

builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

builder.Services.AddSingleton<IHasheadorContrasenas, HasheadorContrasenas>();
builder.Services.AddSingleton<IGeneradorToken, GeneradorToken>();

builder.Services.AddScoped<IAutenticacionServicio, AutenticacionServicio>();
builder.Services.AddScoped<IDocenteServicio, DocenteServicio>();
builder.Services.AddScoped<IEstudianteServicio, EstudianteServicio>();
builder.Services.AddScoped<IArticuloServicio, ArticuloServicio>();
builder.Services.AddScoped<IGrupoServicio, GrupoServicio>();
builder.Services.AddScoped<IEvaluacionServicio, EvaluacionServicio>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = opcionesJwt.Emisor,
            ValidAudience = opcionesJwt.Audiencia,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opcionesJwt.Clave)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo { Title = "Evaluación de artículos JEAC", Version = "v1" });
    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue el token obtenido en /api/autenticacion/login."
    });
    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

using (var alcance = app.Services.CreateScope())
{
    var contexto = alcance.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasheador = alcance.ServiceProvider.GetRequiredService<IHasheadorContrasenas>();
    DatosIniciales.Sembrar(contexto, hasheador);
}

app.UseMiddleware<ManejadorErroresMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
