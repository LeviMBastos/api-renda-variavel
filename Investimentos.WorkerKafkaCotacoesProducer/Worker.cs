using Confluent.Kafka;
using System.Net.Http;
using System.Text.Json;

namespace Investimentos.KafkaCotacoesProducer;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        var httpClient = _httpClientFactory.CreateClient();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await httpClient.GetAsync("https://b3api.vercel.app/api/assets", stoppingToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Erro HTTP: {response.StatusCode}");
                    continue;
                }

                var jsonContent = await response.Content.ReadAsStringAsync(stoppingToken);
                var assets = JsonSerializer.Deserialize<List<B3Asset>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (assets == null || assets.Count == 0)
                {
                    _logger.LogWarning("Nenhum ativo encontrado.");
                    continue;
                }

                foreach (var asset in assets)
                {
                    if (string.IsNullOrEmpty(asset.Ticker) || asset.Price == null || asset.TradeTime == null)
                    {
                        _logger.LogWarning($"❌ Ativo inválido: {asset.Ticker} - {asset.Price} - {asset.TradeTime}");
                        continue;
                    }

                    var payload = new
                    {
                        Ticker = asset.Ticker,
                        Price = asset.Price.Value,
                        TradeTime = asset.TradeTime.Value
                    };

                    var kafkaJson = JsonSerializer.Serialize(payload);
                    await producer.ProduceAsync("cotacoes", new Message<Null, string> { Value = kafkaJson }, stoppingToken);

                    _logger.LogInformation($"✅ Enviado: {payload.Ticker} - {payload.Price} - {payload.TradeTime}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar ou enviar cotações");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // roda a cada 5 minutos
        }
    }
}

public class B3Asset
{
    public string Ticker { get; set; }
    public decimal? Price { get; set; }
    public DateTime? TradeTime { get; set; }
}
