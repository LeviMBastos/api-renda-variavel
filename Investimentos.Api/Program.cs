using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;
using Investimentos.Infra;
using Investimentos.Infra.Repositories;
using Investimentos.Infra.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Conexão com o banco
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<InvestimentosDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Serviços
builder.Services.AddScoped<IOperacaoRepository, OperacaoRepository>();
builder.Services.AddScoped<IOperacaoService, OperacaoService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Para Swagger funcionar corretamente
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Investimentos API", Version = "v1" });
});

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Investimentos API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // Aqui mapeia suas rotas de controllers

app.Run();