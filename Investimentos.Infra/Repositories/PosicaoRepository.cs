using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infra.Repositories
{
    public class PosicaoRepository : IPosicaoRepository
    {
        private readonly InvestimentosDbContext _context;
        public PosicaoRepository(InvestimentosDbContext context)
        {
            _context = context;
        }

        public async Task<Posicao?> GetPosicaoAsync(int usuarioId, int ativoId)
        {
            return await _context.Posicoes
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && p.AtivoId == ativoId);
        }

        public async Task AddAsync(Posicao posicao)
        {
            _context.Posicoes.Add(posicao);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Posicao posicao)
        {
            _context.Posicoes.Update(posicao);
            await _context.SaveChangesAsync();
        }
    }
}
