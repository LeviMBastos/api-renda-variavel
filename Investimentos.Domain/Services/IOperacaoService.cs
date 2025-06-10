using Investimentos.Domain.DTOs;

namespace Investimentos.Domain.Services
{
    public interface IOperacaoService
    {
        Task<List<UsuarioCorretagemDto>> ObterTop10ClientesPorCorretagemAsync();
        Task<UsuarioDto> ObterOperacoesUsuarioAsync(int usuarioId);
        Task<decimal> ObterTotalCorretagemAsync();
        Task<decimal> CalcularPrecoMedioAsync(int ativoId, int usuarioId);
        Task RealizarCompraAsync(OperacaoCompraDto operacaoCompra);
    }
}
