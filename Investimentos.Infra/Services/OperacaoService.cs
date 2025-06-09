using Investimentos.Domain.DTOs;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;
using Investimentos.Infra.Repositories;

namespace Investimentos.Infra.Services
{
    public class OperacaoService : IOperacaoService
    {
        private readonly IOperacaoRepository _operacaoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPosicaoService _posicaoService;

        public OperacaoService(IOperacaoRepository operacaoRepository, 
                               IUsuarioRepository usuarioRepository, 
                               
                               IPosicaoService posicaoService)
        {
            _operacaoRepository = operacaoRepository;
            _usuarioRepository = usuarioRepository;

            _posicaoService = posicaoService;
        }

        public async Task<UsuarioDto> ObterOperacoesUsuarioAsync(int usuarioId)
        {
            List<Operacao> operacoesUsuario = await _operacaoRepository.ObterPorUsuarioAsync(usuarioId);

            return Map(operacoesUsuario);
        }

        public async Task<decimal> CalcularPrecoMedioAsync(int ativoId, int usuarioId)
        {
            List<Operacao>? compras = await _operacaoRepository.GetOperacoesDeCompraAsync(usuarioId, ativoId);

            if (compras == null || !compras.Any())
                throw new ArgumentException("Não há compras para o ativo especificado.");

            decimal totalInvestido = 0;
            int totalQuantidade = 0;

            foreach (Operacao compra in compras)
            {
                totalInvestido += compra.PrecoUnitario * compra.Quantidade;
                totalQuantidade += compra.Quantidade;
            }

            if (totalQuantidade == 0)
                return 0;

            return totalInvestido / totalQuantidade;
        }

        public async Task RealizarCompraAsync(OperacaoCompraDto operacaoCompra)
        {
            if(_usuarioRepository.ObterPorIdAsync(operacaoCompra.UsuarioId) == null)
                throw new ArgumentException("Usuário não encontrado.");

            var operacao = new Operacao
            {
                UsuarioId = operacaoCompra.UsuarioId,
                AtivoId = operacaoCompra.AtivoId,
                Quantidade = operacaoCompra.Quantidade,
                PrecoUnitario = operacaoCompra.PrecoUnitario,
                TipoOperacao = "COMPRA",
                Corretagem = operacaoCompra.Corretagem,
                DataHora = DateTime.Now
            };

            await _operacaoRepository.AddAsync(operacao);

            await _posicaoService.AtualizarPosicaoAposCompraAsync(
                operacaoCompra.UsuarioId,
                operacaoCompra.AtivoId,
                operacaoCompra.PrecoUnitario,
                operacaoCompra.Quantidade
            );
        }

        #region Map

        private static UsuarioDto Map(List<Operacao> operacoes)
        {
            var result = new UsuarioDto
            {
                Operacoes = new List<OperacaoDto>(),
                Posicoes = new List<PosicaoDto>()
            };

            if (operacoes == null || !operacoes.Any())
                return result;

            var usuario = operacoes.First().Usuario;

            result.Id = usuario.Id;
            result.Nome = usuario.Nome;
            result.Email = usuario.Email;
            result.PercentualCorretagem = usuario.PercentualCorretagem;

            result.Operacoes = operacoes.Select(op => new OperacaoDto
            {
                Id = op.Id,
                Quantidade = op.Quantidade,
                PrecoUnitario = op.PrecoUnitario,
                TipoOperacao = op.TipoOperacao.ToString(),
                Corretagem = op.Corretagem,
                DataHora = op.DataHora,
                Ativo = new AtivoDto
                {
                    Id = op.Ativo.Id,
                    Codigo = op.Ativo.Codigo,
                    Nome = op.Ativo.Nome,
                    Cotacao = op.Ativo.Cotacoes?
                        .OrderByDescending(c => c.DataHora)
                        .Select(c => new CotacaoDto
                        {
                            Id = c.Id,
                            PrecoUnitario = c.PrecoUnitario,
                            DataHora = c.DataHora
                        }).FirstOrDefault() ?? new CotacaoDto()
                }
            }).ToList();

            var ativosAgrupados = operacoes
                .GroupBy(o => o.Ativo.Id);

            foreach (var grupo in ativosAgrupados)
            {
                var ativo = grupo.First().Ativo;
                var cotacao = ativo.Cotacoes?
                    .OrderByDescending(c => c.DataHora)
                    .FirstOrDefault();

                int quantidadeTotal = 0;
                decimal custoTotal = 0;

                foreach (var op in grupo.OrderBy(o => o.DataHora))
                {
                    var totalOperacao = op.PrecoUnitario * op.Quantidade;

                    if (op.TipoOperacao == EnumTipoOperacao.COMPRA.ToString())
                    {
                        custoTotal += totalOperacao + op.Corretagem;
                        quantidadeTotal += op.Quantidade;
                    }
                    else if (op.TipoOperacao == EnumTipoOperacao.VENDA.ToString())
                    {
                        var precoMedioAntes = quantidadeTotal > 0 ? custoTotal / quantidadeTotal : 0;
                        custoTotal -= precoMedioAntes * op.Quantidade;
                        quantidadeTotal -= op.Quantidade;
                    }
                }

                decimal precoMedio = quantidadeTotal > 0 ? custoTotal / quantidadeTotal : 0;
                decimal precoAtual = cotacao?.PrecoUnitario ?? 0;
                decimal pl = (precoAtual - precoMedio) * quantidadeTotal;

                result.Posicoes.Add(new PosicaoDto
                {
                    Quantidade = quantidadeTotal,
                    PrecoMedio = Math.Round(precoMedio, 2),
                    PL = Math.Round(pl, 2),
                    Ativo = new AtivoDto
                    {
                        Id = ativo.Id,
                        Codigo = ativo.Codigo,
                        Nome = ativo.Nome,
                        Cotacao = cotacao != null ? new CotacaoDto
                        {
                            Id = cotacao.Id,
                            PrecoUnitario = cotacao.PrecoUnitario,
                            DataHora = cotacao.DataHora
                        } : new CotacaoDto()
                    }
                });
            }

            return result;
        }

        #endregion Map
    }
}
