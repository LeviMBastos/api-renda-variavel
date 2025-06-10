using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infra.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly InvestimentosDbContext _context;
        public UsuarioRepository(InvestimentosDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> ObterPorIdAsync(int id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
