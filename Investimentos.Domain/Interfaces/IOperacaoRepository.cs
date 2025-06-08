using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces
{
    public interface IOperacaoRepository
    {
        Task<List<Operacao>> ObterPorUsuarioAsync(int usuarioId);
    }
}
