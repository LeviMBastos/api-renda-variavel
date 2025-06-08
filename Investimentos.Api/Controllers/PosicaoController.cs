using Microsoft.AspNetCore.Mvc;

namespace Investimentos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PosicaoController : ControllerBase
    {
        //private readonly IPosicaoService _posicaoService;

        //public PosicaoController(IPosicaoService posicaoService)
        //{
        //    _posicaoService = posicaoService;
        //}

        //[HttpGet("{usuarioId}")]
        //public async Task<IActionResult> ObterPosicaoPorUsuario(int usuarioId)
        //{
        //    var result = await _posicaoService.ObterPosicao(usuarioId);
        //    return Ok(result);
        //}
    }
}
