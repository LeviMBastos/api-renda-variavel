using Investimentos.Domain.Interfaces;

namespace Investimentos.Infra.Repositories
{
    public class AtivoRepository : IAtivoRepository
    {
        private readonly InvestimentosDbContext _context;

        public AtivoRepository(InvestimentosDbContext context)
        {
            _context = context;
        }
        
    }
}
