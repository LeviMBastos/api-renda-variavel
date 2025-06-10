using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;
using Investimentos.Infra;
using Investimentos.Infra.Repositories;
using Investimentos.Infra.Services;
using Investimentos.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<CotacaoConsumerWorker>();

builder.Services.AddScoped<IAtivoRepository, AtivoRepository>();
builder.Services.AddScoped<ICotacaoRepository, CotacaoRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICotacaoService, CotacaoService>();

// Também registre seu DbContext (se ainda não fez):
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<InvestimentosDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);


var host = builder.Build();
host.Run();
