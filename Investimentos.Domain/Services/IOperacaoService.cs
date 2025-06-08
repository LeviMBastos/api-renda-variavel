using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Services
{
    public interface IOperacaoService
    {
        Task<List<Operacao>> ObterOperacoesUsuarioAsync(int usuarioId);
    }
}
