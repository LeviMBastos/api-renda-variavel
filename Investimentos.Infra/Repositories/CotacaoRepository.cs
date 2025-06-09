using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infra.Repositories
{
    public class CotacaoRepository : ICotacaoRepository
    {
        private readonly InvestimentosDbContext _context;

        public CotacaoRepository(InvestimentosDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteCotacaoAsync(int ativoId, DateTime dataHora)
        {
            return await _context.Cotacoes
                .AnyAsync(c => c.AtivoId == ativoId && c.DataHora == dataHora);
        }

        public async Task AdicionarAsync(Cotacao cotacao)
        {
            await _context.Cotacoes.AddAsync(cotacao);
        }
    }
}
