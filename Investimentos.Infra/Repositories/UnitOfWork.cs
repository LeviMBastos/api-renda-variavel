using Investimentos.Domain.Interfaces;
using System;

namespace Investimentos.Infra.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InvestimentosDbContext _context;

        public IAtivoRepository Ativos { get; }
        public ICotacaoRepository Cotacoes { get; }

        public UnitOfWork(InvestimentosDbContext context,
                          IAtivoRepository ativoRepository,
                          ICotacaoRepository cotacaoRepository)
        {
            _context = context;
            Ativos = ativoRepository;
            Cotacoes = cotacaoRepository;
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
