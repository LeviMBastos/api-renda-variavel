using Confluent.Kafka;
using Investimentos.Domain.Services;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Wrap;

namespace Investimentos.Worker
{
    public class CotacaoConsumerWorker : BackgroundService
    {
        private readonly ILogger<CotacaoConsumerWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AsyncPolicyWrap _policy;

        public CotacaoConsumerWorker(ILogger<CotacaoConsumerWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;

            // Fallback para ignorar erros do CotacaoService
            var fallbackPolicy = Policy
                .Handle<Exception>()
                .FallbackAsync(async (ct) =>
                {
                    _logger.LogWarning("⚠️ Fallback executado: falha ao processar cotação Kafka.");
                });

            // Circuit breaker para proteger banco/repositório
            var circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(30),
                    minimumThroughput: 2,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (ex, breakDelay) =>
                    {
                        _logger.LogWarning("⛔ Circuit breaker aberto por {Seconds}s devido a erro: {Erro}", breakDelay.TotalSeconds, ex.Message);
                    },
                    onReset: () => _logger.LogInformation("🔄 Circuit breaker resetado."),
                    onHalfOpen: () => _logger.LogInformation("⚠️ Circuit breaker em estado half-open.")
                );

            _policy = Policy.WrapAsync(fallbackPolicy, circuitBreakerPolicy);
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

                        await _policy.ExecuteAsync(async () =>
                        {
                            await cotacaoService.ProcessarCotacaoKafkaAsync(message);
                        });

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
