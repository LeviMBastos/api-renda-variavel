using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infra.Repositories
{
    public class AtivoRepository : IAtivoRepository
    {
        private readonly InvestimentosDbContext _context;

        public AtivoRepository(InvestimentosDbContext context)
        {
            _context = context;
        }

        public async Task<Ativo?> BuscarPorCodigoAsync(string codigo)
        {
            return await _context.Ativos
                .Include(a => a.Cotacoes)
                .FirstOrDefaultAsync(a => a.Codigo == codigo);
        }

        public async Task AdicionarAsync(Ativo ativo)
        {
            await _context.Ativos.AddAsync(ativo);
        }
    }
}
