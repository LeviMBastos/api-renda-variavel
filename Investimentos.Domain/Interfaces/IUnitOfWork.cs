
namespace Investimentos.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAtivoRepository Ativos { get; }
        ICotacaoRepository Cotacoes { get; }

        Task<int> CommitAsync();
    }
}
