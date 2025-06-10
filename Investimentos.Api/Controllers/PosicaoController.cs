using Investimentos.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PosicaoController : ControllerBase
    {
        private readonly IPosicaoService _posicaoService;

        public PosicaoController(IPosicaoService posicaoService)
        {
            _posicaoService = posicaoService;
        }

        [HttpGet("getTop10ClientesPorPl")]
        public async Task<IActionResult> ObterTop10ClientesPorPl()
        {
            var result = await _posicaoService.ObterTop10ClientesPorPlAsync();
            return Ok(result);
        }
    }
}
