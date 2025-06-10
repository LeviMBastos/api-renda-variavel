
using Investimentos.Domain.DTOs;
using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Services
{
    public interface IUsuarioService
    {
        Task<Usuario> CriarAsync(UsuarioCriacaoDto dto);
        Task<UsuarioDto?> ObterPorIdAsync(int id);
    }
}
