using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorIdAsync(int id);
    }
}
