using Investimentos.Domain.DTOs;
using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces
{
    public interface IOperacaoRepository
    {
        Task<List<Operacao>> ObterPorUsuarioAsync(int usuarioId);
        Task<List<Operacao>?> GetOperacoesDeCompraAsync(int usuarioId, int ativoId);
        Task AddAsync(Operacao operacao);
    }
}
