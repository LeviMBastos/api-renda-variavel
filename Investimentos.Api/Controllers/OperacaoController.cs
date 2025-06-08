using Investimentos.Domain.Services;
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
    }
}
