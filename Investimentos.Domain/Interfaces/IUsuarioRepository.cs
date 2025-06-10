using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task AdicionarAsync(Usuario usuario);
        Task<Usuario?> ObterPorIdAsync(int id);
    }
}
