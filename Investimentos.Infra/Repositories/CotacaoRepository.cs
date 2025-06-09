using Investimentos.Domain.Interfaces;

namespace Investimentos.Infra.Repositories
{
    public class CotacaoRepository : ICotacaoRepository
    {
        private readonly InvestimentosDbContext _context;

        public CotacaoRepository(InvestimentosDbContext context)
        {
            _context = context;
        }
    }
}
