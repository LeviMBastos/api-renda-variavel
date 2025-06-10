using Investimentos.Domain.DTOs;
using Investimentos.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarUsuario([FromBody] UsuarioCriacaoDto dto)
        {
            var usuarioCriado = await _usuarioService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = usuarioCriado.Id }, usuarioCriado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }
    }
}
