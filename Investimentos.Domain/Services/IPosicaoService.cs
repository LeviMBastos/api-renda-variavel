
namespace Investimentos.Domain.Services
{
    public interface IPosicaoService
    {
        Task AtualizarPosicaoAposCompraAsync(int usuarioId, int ativoId, decimal precoCompra, int qtdCompra);
    }
}
