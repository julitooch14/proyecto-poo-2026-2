using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Servicios;
using Servicios.Interfaces;
using Servicios.Servicios;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Capa de datos (base de datos en memoria). Se registra a través del proyecto Servicios
// para que el API no conozca directamente la capa de Datos.
builder.Services.AgregarBaseDeDatos();

// Inyección de dependencias: cada interfaz se resuelve con su servicio.
builder.Services.AddTransient<IAutenticacion, ServicioAutenticacion>();
builder.Services.AddTransient<IBloqueExposicion, ServicioBloqueExposicion>();
builder.Services.AddTransient<ICatalogo, ServicioCatalogo>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("No se configuró Jwt:Key.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
        };
    });
builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Crea la base de datos en memoria y carga los datos de prueba definidos
// en OnModelCreating.
app.Services.InicializarBaseDeDatos();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Sirve el front end (carpeta wwwroot) desde el mismo servidor que la API.
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
