using Investimentos.Domain.DTOs;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Domain.Services;

namespace Investimentos.Infra.Services
{
    public class OperacaoService : IOperacaoService
    {
        private readonly IOperacaoRepository _repo;

        public OperacaoService(IOperacaoRepository repo)
        {
            _repo = repo;
        }

        public async Task<UsuarioDto> ObterOperacoesUsuarioAsync(int usuarioId)
        {
            List<Operacao> operacoesUsuario = await _repo.ObterPorUsuarioAsync(usuarioId);

            return Map(operacoesUsuario);
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

            // Mapear operações
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

            // Agrupar por ativo para gerar posições
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
