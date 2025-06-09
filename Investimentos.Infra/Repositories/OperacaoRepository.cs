using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infra.Repositories
{
    public class OperacaoRepository : IOperacaoRepository
    {
        private readonly InvestimentosDbContext _context;

        public OperacaoRepository(InvestimentosDbContext context)
        {
            _context = context;
        }

        public async Task<List<Operacao>> ObterPorUsuarioAsync(int usuarioId)
        {
            return await _context.Operacoes
                .Include(o => o.Ativo).Include(o => o.Usuario)
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.DataHora)
                .ToListAsync();
        }

        public async Task<List<Operacao>?> GetOperacoesDeCompraAsync(int usuarioId, int ativoId)
        {
            List<Operacao>? operacoesDeCompra = await _context.Operacoes
                                                             .Where(o => o.UsuarioId == usuarioId &&
                                                                         o.AtivoId == ativoId &&
                                                                         o.TipoOperacao == "COMPRA")
                                                             .OrderBy(o => o.DataHora)
                                                             .ToListAsync();

            return operacoesDeCompra;
        }

        public async Task AddAsync(Operacao operacao)
        {
            _context.Operacoes.Add(operacao);
            await _context.SaveChangesAsync();
        }
    }
}
