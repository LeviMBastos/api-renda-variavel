using Investimentos.Domain.DTOs;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Investimentos.Infra.Services
{
    public class CotacaoService : ICotacaoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CotacaoService> _logger;

        public CotacaoService(IUnitOfWork unitOfWork, ILogger<CotacaoService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ProcessarCotacaoKafkaAsync(string mensagem)
        {
            CotacaoKafkaDto? cotacaoKafka;

            try
            {
                cotacaoKafka = JsonSerializer.Deserialize<CotacaoKafkaDto>(mensagem, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Erro ao desserializar mensagem Kafka: {Mensagem}", mensagem);
                return;
            }

            if (cotacaoKafka == null || string.IsNullOrWhiteSpace(cotacaoKafka.Ticker))
            {
                _logger.LogWarning("⚠️ Mensagem inválida recebida: {Mensagem}", mensagem);
                return;
            }

            var ativo = await _unitOfWork.Ativos.BuscarPorCodigoAsync(cotacaoKafka.Ticker);
            if (ativo == null)
            {
                ativo = new Ativo
                {
                    Codigo = cotacaoKafka.Ticker,
                    Nome = cotacaoKafka.Ticker
                };
                await _unitOfWork.Ativos.AdicionarAsync(ativo);
                await _unitOfWork.CommitAsync(); // Garante Id do Ativo
            }

            var jaExiste = await _unitOfWork.Cotacoes.ExisteCotacaoAsync(ativo.Id, cotacaoKafka.TradeTime);
            if (jaExiste)
            {
                _logger.LogInformation("ℹ️ Cotação já existente para ativo {Ticker} em {DataHora}", cotacaoKafka.Ticker, cotacaoKafka.TradeTime);
                return;
            }

            var cotacao = new Cotacao
            {
                AtivoId = ativo.Id,
                PrecoUnitario = cotacaoKafka.Price,
                DataHora = cotacaoKafka.TradeTime
            };

            await _unitOfWork.Cotacoes.AdicionarAsync(cotacao);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("💾 Cotação salva: {Ticker} - R$ {Preco} em {DataHora}", cotacaoKafka.Ticker, cotacaoKafka.Price, cotacaoKafka.TradeTime);
        }
    }
}
