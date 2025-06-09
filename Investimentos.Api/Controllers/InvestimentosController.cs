using Investimentos.Domain.DTOs;
using Investimentos.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvestimentosController : ControllerBase
    {
        private readonly IPosicaoService _posicaoService;
        private readonly IOperacaoService _operacaoService;

        public InvestimentosController(IPosicaoService posicaoService, IOperacaoService operacaoService)
        {
            _posicaoService = posicaoService;
            _operacaoService = operacaoService;
        }

        [HttpPost("comprar")]
        public async Task<IActionResult> ComprarAtivo([FromBody] OperacaoCompraDto operacaoCompra)
        {
            try
            {
                await _operacaoService.RealizarCompraAsync(operacaoCompra);
                return Ok("Compra realizada com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("preco-medio")]
        public async Task<IActionResult> GetPrecoMedio(int usuarioId, int ativoId)
        {
            try
            {
                var precoMedio = await _operacaoService.CalcularPrecoMedioAsync(ativoId, usuarioId);
                return Ok(precoMedio);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
