using Investimentos.Domain.Services;
using Investimentos.Infra.Services;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtivoController : ControllerBase
    {
        private readonly IAtivoService _ativoService;

        public AtivoController(IAtivoService ativoService)
        {
            _ativoService = ativoService;
        }

        [HttpGet("ObterUltimaCotacao/{codigoAtivo}")]
        public async Task<IActionResult> ObterUltimaCotacao(string codigoAtivo)
        {
            var cotacao = await _ativoService.ObterUltimaCotacaoAsync(codigoAtivo);

            if (cotacao == null)
                return NotFound();

            return Ok(cotacao);
        }
    }
}
