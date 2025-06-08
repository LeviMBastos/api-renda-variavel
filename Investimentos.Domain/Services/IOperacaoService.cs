using Investimentos.Domain.DTOs;

namespace Investimentos.Domain.Services
{
    public interface IOperacaoService
    {
        Task<UsuarioDto> ObterOperacoesUsuarioAsync(int usuarioId);
    }
}
