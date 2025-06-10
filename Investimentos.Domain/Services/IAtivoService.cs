
using Investimentos.Domain.DTOs;

namespace Investimentos.Domain.Services
{
    public interface IAtivoService
    {
        Task<AtivoDto?> ObterUltimaCotacaoAsync(string codigoAtivo);
    }
}
