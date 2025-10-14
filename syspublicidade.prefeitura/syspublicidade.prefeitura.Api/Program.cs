using Cortex.Mediator.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using syspublicidade.prefeitura.Domain.Entities;
using syspublicidade.prefeitura.Infrastructure;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("🚀 Iniciando syspublicidade.prefeitura.API...");

// ==============================
// ✅ DATABASE IN-MEMORY (para testes)
// ==============================
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("FakeDb"));

// ==============================
// ✅ CORS (Frontend localhost dev)
// ==============================
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("Frontend", p =>
        p.WithOrigins("http://localhost:5173", "https://localhost:5173", "https://sgm-front.vercel.app", "https://www.sevenfullstack.com.br")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;             // força URLs minúsculas
    options.LowercaseQueryStrings = true;     // opcional: também força querystrings minúsculas
});
// ==============================
// ✅ JWT CONFIG
// ==============================
var jwtKey = builder.Configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("Jwt__Key");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("Jwt__Issuer");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("Jwt__Audience");

// Log para verificação no Azure (não mostra a key)
Console.WriteLine($"🔐 JWT Variáveis -> Key? {(string.IsNullOrEmpty(jwtKey) ? "❌" : "✅")} | Issuer={jwtIssuer} | Audience={jwtAudience}");

// Fallback seguro se faltar a key
if (string.IsNullOrEmpty(jwtKey))
{
    Console.WriteLine("⚠️ Atenção: Jwt:Key não configurada! Usando fallback temporário.");
    jwtKey = "temporary-fallback-key";
}

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"[JWT] Auth failed: {ctx.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = ctx =>
            {
                Console.WriteLine($"[JWT] Challenge: {ctx.Error} | {ctx.ErrorDescription}");
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                Console.WriteLine("[JWT] Token OK");
                return Task.CompletedTask;
            }
        };
    });

// ==============================
// ✅ SERVICES / CONTROLLERS / SWAGGER
// ==============================
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Cortex Mediator registration
builder.Services.AddCortexMediator(
    builder.Configuration,
    new[] { typeof(syspublicidade.prefeitura.Application.AssemblyMarker) }
);

// ==============================
// ✅ BUILD APP
// ==============================
var app = builder.Build();

// ==============================
// ✅ SEED ADMIN (apenas se DB vazio)
// ==============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    if (!db.Usuarios.Any())
    {
        var adminId = Guid.NewGuid();
        db.Usuarios.Add(new Usuarios
        {
            Id = adminId,
            Nome = "Giovani Godinho da Silva",
            Email = "admin@prefeitura.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            IsAdmin = true
        });
        db.SaveChanges();
        Console.WriteLine($"👑 Admin criado: {adminId} | admin@prefeitura.com / 123456");
    }
}

// ==============================
// ✅ PIPELINE ORDER
// ==============================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = string.Empty;
});

// ❌ Não use app.UseHttpsRedirection() dentro do container Linux
app.UseRouting();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

// ==============================
// ✅ HEALTHCHECKS & ROOT ROUTE
// ==============================
app.MapGet("/", () => Results.Ok("✅ syspublicidade.prefeitura.API online"));
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
