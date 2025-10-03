using Microsoft.EntityFrameworkCore;
using syspublicidade.prefeitura.Infrastructure;
using Cortex.Mediator.DependencyInjection;
using syspublicidade.prefeitura.Application.features.NoticiasM.Handlers; // marker
using syspublicidade.prefeitura.Application.features.UsuariosM.Handlers; // marker opcional

var builder = WebApplication.CreateBuilder(args);

// Banco em memória
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("FakeDb"));

// Infra de API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Registrar Cortex.Mediator, escaneando a camada Application inteira
builder.Services.AddCortexMediator(
    builder.Configuration,
    new[] { typeof(syspublicidade.prefeitura.Application.AssemblyMarker) } // markers
);

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

try
{
    app.Run();
}
catch
{
    Thread.Sleep(Timeout.Infinite);
}
