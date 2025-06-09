using Confluent.Kafka;
using Investimentos.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Investimentos.Worker
{
    public class CotacaoConsumerWorker : BackgroundService
    {
        private readonly ILogger<CotacaoConsumerWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public CotacaoConsumerWorker(ILogger<CotacaoConsumerWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "cotacoes-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe("cotacoes");

            _logger.LogInformation("🚀 CotacaoConsumerWorker iniciado e escutando o tópico 'cotacoes'...");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        var message = consumeResult?.Message?.Value;

                        if (string.IsNullOrWhiteSpace(message))
                        {
                            _logger.LogWarning("⚠️ Mensagem vazia ou nula recebida do Kafka.");
                            continue;
                        }

                        using var scope = _scopeFactory.CreateScope();
                        var cotacaoService = scope.ServiceProvider.GetRequiredService<ICotacaoService>();

                        await cotacaoService.ProcessarCotacaoKafkaAsync(message);

                        _logger.LogInformation("✅ Cotação processada com sucesso.");
                    }
                    catch (ConsumeException cex)
                    {
                        _logger.LogError(cex, "Erro ao consumir mensagem do Kafka.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro inesperado ao processar mensagem Kafka.");
                        await Task.Delay(3000, stoppingToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("🛑 CotacaoConsumerWorker cancelado.");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
