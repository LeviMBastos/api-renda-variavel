using Investimentos.Domain.DTOs;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;

namespace Investimentos.Infra.Services
{
    public class AtivoService : IAtivoService
    {
        private readonly IAtivoRepository _ativoRepository;

        public AtivoService(IAtivoRepository ativoRepository)
        {
            _ativoRepository = ativoRepository;
        }

        public async Task<AtivoDto?> ObterUltimaCotacaoAsync(string codigoAtivo)
        {
            var ativo = await _ativoRepository.BuscarPorCodigoAsync(codigoAtivo);

            if (ativo == null || ativo.Cotacoes == null || !ativo.Cotacoes.Any())
                return null;

            var ultimaCotacao = ativo.Cotacoes.OrderByDescending(c => c.DataHora).First();

            return new AtivoDto
            {
                Id = ativo.Id,
                Codigo = ativo.Codigo,
                Nome = ativo.Nome,
                Cotacao = new CotacaoDto
                {
                    Id = ultimaCotacao.Id,
                    PrecoUnitario = ultimaCotacao.PrecoUnitario,
                    DataHora = ultimaCotacao.DataHora
                }
            };
        }
    }
}
