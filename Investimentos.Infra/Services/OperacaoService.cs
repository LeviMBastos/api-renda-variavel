using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;

namespace Investimentos.Infra.Services
{
    public class OperacaoService : IOperacaoService
    {
        private readonly IOperacaoRepository _repo;

        public OperacaoService(IOperacaoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Operacao>> ObterOperacoesUsuarioAsync(int usuarioId)
        {
            return await _repo.ObterPorUsuarioAsync(usuarioId);
        }
    }
}
