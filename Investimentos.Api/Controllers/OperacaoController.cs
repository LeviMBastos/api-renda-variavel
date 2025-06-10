using Investimentos.Domain.Services;
using Investimentos.Infra.Services;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperacaoController : ControllerBase
    {
        private readonly IOperacaoService _service;

        public OperacaoController(IOperacaoService service)
        {
            _service = service;
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> GetOperacoes(int usuarioId)
        {
            var operacoes = await _service.ObterOperacoesUsuarioAsync(usuarioId);
            return Ok(operacoes);
        }

        [HttpGet("getTotalCorretagens")]
        public async Task<IActionResult> ObterTotalCorretagens()
        {
            var total = await _service.ObterTotalCorretagemAsync();
            return Ok(new { TotalCorretagens = total });
        }

        [HttpGet("getTop10ClientesPorCorretagemAsync")]
        public async Task<IActionResult> ObterTop10ClientesPorCorretagem()
        {
            var result = await _service.ObterTop10ClientesPorCorretagemAsync();
            return Ok(result);
        }
    }
}
