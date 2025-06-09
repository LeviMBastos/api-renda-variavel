
using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces
{
    public interface ICotacaoRepository
    {
        Task<bool> ExisteCotacaoAsync(int ativoId, DateTime dataHora);
        Task AdicionarAsync(Cotacao cotacao);
    }
}
