using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;

namespace Investimentos.Infra.Services
{
    public class PosicaoService : IPosicaoService
    {
        private readonly IPosicaoRepository _posicaoRepository;
        private readonly IOperacaoRepository _operacaoRepository;

        public PosicaoService(IPosicaoRepository posicaoRepository, IOperacaoRepository operacaoRepository)
        {
            _posicaoRepository = posicaoRepository;
            _operacaoRepository = operacaoRepository;
        }

        public async Task AtualizarPosicaoAposCompraAsync(int usuarioId, int ativoId, decimal precoCompra, int qtdCompra)
        {
            var posicao = await _posicaoRepository.GetPosicaoAsync(usuarioId, ativoId);

            if (posicao == null)
            {
                posicao = new Posicao
                {
                    UsuarioId = usuarioId,
                    AtivoId = ativoId,
                    Quantidade = qtdCompra,
                    PrecoMedio = precoCompra,
                    PL = 0
                };

                await _posicaoRepository.AddAsync(posicao);
            }
            else
            {
                decimal novoPrecoMedio = (posicao.Quantidade * posicao.PrecoMedio + qtdCompra * precoCompra) /
                                          (posicao.Quantidade + qtdCompra);

                posicao.Quantidade += qtdCompra;
                posicao.PrecoMedio = novoPrecoMedio;

                await _posicaoRepository.UpdateAsync(posicao);
            }
        }
    }
}
