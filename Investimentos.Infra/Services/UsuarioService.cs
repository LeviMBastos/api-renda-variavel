using Investimentos.Domain.DTOs;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;

namespace Investimentos.Infra.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario> CriarAsync(UsuarioCriacaoDto dto)
        {
            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                PercentualCorretagem = dto.PercentualCorretagem
            };

            await _usuarioRepository.AdicionarAsync(usuario);

            return usuario;
        }

        public async Task<UsuarioDto?> ObterPorIdAsync(int id)
        {
            var usuario = await _usuarioRepository.ObterPorIdAsync(id);
            
            if (usuario == null)
                return null;

            return Map(usuario);
        }

        #region Map
        
        private static UsuarioDto Map(Usuario usuario)
        {
            if (usuario == null)
                return null;

            return new UsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                PercentualCorretagem = usuario.PercentualCorretagem
            };
        }

        #endregion Map
    }
}
