
using Investimentos.Domain.DTOs;

namespace Investimentos.Domain.Services
{
    public interface IPosicaoService
    {
        Task<List<UsuarioPlDto>> ObterTop10ClientesPorPlAsync();
        Task AtualizarPosicaoAposCompraAsync(int usuarioId, int ativoId, decimal precoCompra, int qtdCompra);
    }
}
