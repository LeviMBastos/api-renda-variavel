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
                .Include(o => o.Ativo)
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.DataHora)
                .ToListAsync();
        }
    }
}
