
using Investimentos.Domain.DTOs;

namespace Investimentos.Domain.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioDto?> ObterPorIdAsync(int id);
    }
}
