using Investimentos.Domain.DTOs;
using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces
{
    public interface IOperacaoRepository
    {
        Task<decimal> SomarCorretagensAsync();
        Task<List<UsuarioCorretagemDto>> ObterTop10ClientesPorCorretagemAsync();
        Task<List<Operacao>> ObterPorUsuarioAsync(int usuarioId);
        Task<List<Operacao>?> GetOperacoesDeCompraAsync(int usuarioId, int ativoId);
        Task AddAsync(Operacao operacao);
    }
}
